using UnityEngine;

public class PatrolEdge : MonoBehaviour
{
    [Header("Platform and Target Edge")]
    public PatrollingPlatform platform;
    public Transform newTargetEdge;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == platform.transform)
            ((PatrollingPlatform)platform).ChangeDirection(newTargetEdge);
    }
}