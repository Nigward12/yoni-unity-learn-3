using System.Collections;
using UnityEngine;

public class PatrollingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 10f;
    public float slowDownDistance = 6f;
    public float directionChangePauseTime = 3f;

    [Header("Target")]
    public Transform nextPatrolEdge;

    protected bool isPaused = false;
    private bool isChangingDirection = false;
    private Vector2 currentVelocity;

    private void Update()
    {
        if (isPaused || nextPatrolEdge == null)
            return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = nextPatrolEdge.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        float distance = Vector2.Distance(currentPosition, targetPosition);
        float speed = Mathf.Lerp(0f, maxSpeed, Mathf.Clamp01(distance / slowDownDistance));

        Vector2 movement = direction * speed * Time.deltaTime;
        transform.Translate(movement);

        currentVelocity = direction * speed;
    }

    public void ChangeDirection(Transform newEdge)
    {
        if (!isChangingDirection)
            StartCoroutine(DelayedDirectionChange(newEdge));
    }

    private IEnumerator DelayedDirectionChange(Transform newEdge)
    {
        isChangingDirection = true;
        isPaused = true;

        yield return new WaitForSeconds(directionChangePauseTime);

        nextPatrolEdge = newEdge;
        isPaused = false;
        isChangingDirection = false;
    }
}