using UnityEngine;

public abstract class EnemyPatrol : MonoBehaviour
{
    protected bool returningToPatrol;

    public abstract void OnReachedPatrolEdge();

    public void ReturnToPatrol()
    {
        returningToPatrol = true;
        this.enabled = true;
    }

    public bool isReturningToPatrol()
    {
        return returningToPatrol;
    }
}
