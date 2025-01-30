using Unity.VisualScripting;
using UnityEngine;

public class EnemyMeleeMovement : MovementScript
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    private float attackCooldownTimer = Mathf.Infinity;

    [Header("attack range")]
    [SerializeField] private BoxCollider2D attackTriggerCollider;

    [Header("chase settings")]
    [SerializeField] private BoxCollider2D detectionColliderFront;
    [SerializeField] private BoxCollider2D detectionColliderBack;
    [SerializeField] private float stopChaseDistance;
    [SerializeField] private float chaseFadeoutTime;
    [SerializeField] float speed;
    [SerializeField] Transform player;
    private float chaseFadeoutTimer = Mathf.Infinity;

    private BoxCollider2D DetectionTriggerCollider;

    [Header("relevant layers")]
    [SerializeField] private LayerMask playerLayer;

    private Animator anim;
    private EnemyPatrol enemyPatrol;
    private Rigidbody2D body;
    private bool chasing;

    protected override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    protected override void Update()
    {
        base.Update();

        attackCooldownTimer += Time.deltaTime;

        if (PlayerInAttackRange())
        {
            if (attackCooldownTimer >= attackCooldown)
            {
                anim.SetBool("run", false);
                attackCooldownTimer = 0;
                anim.SetTrigger("attack");
            }
        }


        if (!PlayerInAttackRange())
        {
            if (PlayerDetected())
            {
                enemyPatrol.enabled = false;
                ChasePlayer();
            }
            else if (chasing)
                ChaseFadeOut();
            else
                enemyPatrol.enabled = true;
        }
    }

    private void ChasePlayer()
    {
        anim.SetBool("run", true);
        chasing = true;
        chaseFadeoutTimer = 0;

        if (transform.position.x > player.position.x)
        {
            if (!facingLeft)
            {
                TurnOnXAxis();
            }
            body.linearVelocity = new Vector2(-1 * speed, 0f);
        }
        else if (transform.position.x < player.position.x)
        {
            if (facingLeft)
            {
                TurnOnXAxis();
            }
            body.linearVelocity = new Vector2(speed, 0f);
        }
        else
        {
            TurnOnXAxis();
            body.linearVelocity = new Vector2(0f, 0f);
        }

    }

    private void ChaseFadeOut()
    {
        anim.SetBool("run", false);

        chaseFadeoutTimer += Time.deltaTime;

        if (chaseFadeoutTimer > chaseFadeoutTime)
        {
            chasing = false;
            enemyPatrol.ReturnToPatrol();
        }
    }
    private bool PlayerDetected()
    {
        return PlayerInRange(detectionColliderFront) || PlayerInRange(detectionColliderBack);
    }
    private bool PlayerInAttackRange()
    {
        return PlayerInRange(attackTriggerCollider);
    }

    private bool PlayerInRange(BoxCollider2D range)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(range.bounds.center,
            range.bounds.size, 0, playerLayer);

        return hits.Length > 0;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
    //        new Vector2(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y));
    //}

}