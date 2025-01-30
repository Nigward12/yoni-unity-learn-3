using UnityEngine;
using System.Collections;

public class EnemyPatrolSideways : EnemyPatrol
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    [SerializeField] private string patrolMovementAnimVar;
    private Vector3 initScale;
    private bool movingLeft;
    private int movingDirection;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;

    [Header("Relevant Layers")]
    [SerializeField] int enemyLayerCode;
    [SerializeField] int edgeLayerCode;

    private Animator anim;
    private Rigidbody2D body;
    private MovementScript enemyMovement;
    //private float patrolCenterX;

    private void Awake()
    {
        initScale = transform.localScale;
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<MovementScript>();
        //patrolCenterX = (rightEdge.position.x - leftEdge.position.x) / 2;
        returningToPatrol = false;
    }

    private void Start()
    {
        movingLeft = enemyMovement.isFacingLeft();
        movingDirection = 0;
        startPatrol();
    }
    private void OnDisable()
    {
        anim.SetBool(patrolMovementAnimVar, false);
        StopAllCoroutines();
        Physics2D.IgnoreLayerCollision(edgeLayerCode, enemyLayerCode, true);
    }

    private void FixedUpdate()
    { 
        body.linearVelocity = new Vector2(movingDirection * speed, 0f);

        if (returningToPatrol)
        {
            if (transform.position.x >= leftEdge.position.x
                && transform.position.x <= rightEdge.position.x)
                returningToPatrol = false;
        }
    }

    private void OnEnable()
    {
        Physics2D.IgnoreLayerCollision(edgeLayerCode, enemyLayerCode, false);
        Start();
    }


    private void startPatrol()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftEdge.position.x)
                MoveInDirection(enemyMovement.getLeftScaleDirection(), -1);
            else
                StartCoroutine(DirectionChange());
        }
        else
        {
            if (transform.position.x < rightEdge.position.x)
                MoveInDirection(-enemyMovement.getLeftScaleDirection(), 1);
            else
                StartCoroutine(DirectionChange());
        }
    }

    public override void OnReachedPatrolEdge()
    {
        StartCoroutine(DirectionChange());
    }

    private IEnumerator DirectionChange()
    {
        anim.SetBool(patrolMovementAnimVar, false);
        movingDirection = 0;
        yield return new WaitForSeconds(idleDuration);

        movingLeft = !movingLeft;

        if (movingLeft)
        {
            MoveInDirection(enemyMovement.getLeftScaleDirection(), -1);
        }
        else
        {
            MoveInDirection(-enemyMovement.getLeftScaleDirection(), 1);
        }
    }

    private void MoveInDirection(int ScaleDirection, int _movingDirection)
    {
        anim.SetBool(patrolMovementAnimVar, true);

        transform.localScale = new Vector3(Mathf.Abs(initScale.x) * ScaleDirection,
            initScale.y, initScale.z);

        movingDirection = _movingDirection;
      
    }
}