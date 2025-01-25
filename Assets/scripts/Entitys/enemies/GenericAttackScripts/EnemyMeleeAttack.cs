using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    private float attackCooldownTimer = Mathf.Infinity;

    [Header("attack range")]
    [SerializeField] private Transform attackTriggerRange;
     private BoxCollider2D attackTriggerCollider;

    [Header("detection range")]
    [SerializeField] private Transform detectionTriggerRange;
    [SerializeField] private float detectionCooldown;
    private float detectionCooldownTimer = Mathf.Infinity;

    private BoxCollider2D DetectionTriggerCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    private Animator anim;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        attackTriggerCollider = attackTriggerRange.GetComponent<BoxCollider2D>();
        DetectionTriggerCollider = detectionTriggerRange.GetComponent<BoxCollider2D>();
        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    private void Update()
    {
        attackCooldownTimer += Time.deltaTime;
        detectionCooldownTimer += Time.deltaTime;

        if (PlayerInAttackRange())
        {
            if (attackCooldownTimer >= attackCooldown)
            {
                attackCooldownTimer = 0;
                anim.SetTrigger("attack");
            }
        }

        enemyPatrol.enabled = !PlayerInAttackRange();

        // enable/disable enemy chase script
    }

    private bool PlayerDetected()
    {
        return PlayerInRange(DetectionTriggerCollider) || detectionCooldownTimer < detectionCooldown;
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