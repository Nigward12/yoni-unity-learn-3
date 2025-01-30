using UnityEngine;

public class EnemyPatrolEdge : MonoBehaviour
{
    [SerializeField] private Transform patrolEnemy;
    private EnemyPatrol enemyPatrol;
    private void Awake()
    {
        enemyPatrol = patrolEnemy.GetComponent<EnemyPatrol>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform == patrolEnemy && enemyPatrol.enabled && !enemyPatrol.isReturningToPatrol())
        {
            enemyPatrol.OnReachedPatrolEdge();
        }
    }
}
