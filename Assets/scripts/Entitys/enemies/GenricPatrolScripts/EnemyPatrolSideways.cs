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
    [SerializeField] private bool facingLeftOnStart;
    private Vector3 initScale;
    private bool movingLeft;
    private int leftScaleDirection;
    private int movingDirection;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;

    [Header("Relevant Layers")]
    [SerializeField] int enemyLayerCode;
    [SerializeField] int edgeLayerCode;

    private Animator anim;
    private Rigidbody2D body;

    private void Awake()
    {
        initScale = transform.localScale;
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        movingLeft = facingLeftOnStart;
        leftScaleDirection = (int)Mathf.Sign(initScale.x)*1;
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
    }

    private void OnEnable()
    {
        startPatrol();
        Physics2D.IgnoreLayerCollision(edgeLayerCode, enemyLayerCode, false);
    }


    private void startPatrol()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftEdge.position.x)
                MoveInDirection(leftScaleDirection, -1);
            else
                DirectionChange();
        }
        else
        {
            if (transform.position.x < rightEdge.position.x)
                MoveInDirection(-leftScaleDirection, 1);
            else
                DirectionChange();
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
            MoveInDirection(leftScaleDirection, -1);
        }
        else
        {
            MoveInDirection(-leftScaleDirection, 1);
        }
    }

    private void MoveInDirection(int ScaleDirection, int _movingDirection)
    {
        anim.SetBool(patrolMovementAnimVar, true);

        //Make enemy face direction
        transform.localScale = new Vector3(Mathf.Abs(initScale.x) * ScaleDirection,
            initScale.y, initScale.z);

        //Move in that direction
        //body.linearVelocity = new Vector2(_movingDirection * speed, 0f);
        movingDirection = _movingDirection;
        //transform.position = new Vector3(transform.position.x + Time.deltaTime * movingDirection * speed,
        //    transform.position.y, transform.position.z);
    }
}