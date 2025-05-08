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

    protected int pauseCounter = 0;
    protected bool isPaused = false;
    private bool isChangingDirection = false;
    private Vector2 currentVelocity;

    private void Update()
    {
        isPaused = pauseCounter > 0;

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
            DelayedDirectionChange(newEdge);
    }

    private void DelayedDirectionChange(Transform newEdge)
    {
        isChangingDirection = true;
        nextPatrolEdge = newEdge;
        StartCoroutine(PauseForSeconds(directionChangePauseTime));
        isChangingDirection = false;
    }

    public IEnumerator PauseForSeconds(float time)
    {
        Pause();
        yield return new WaitForSeconds(time);
        Resume();
    }

    protected void Pause()
    {
        pauseCounter++;
    }

    protected void Resume()
    {
        pauseCounter--;
        if (pauseCounter < 0) pauseCounter = 0;
    }
}