using UnityEngine;
using System.Collections;

public class beamController : MonoBehaviour
{
    [Header("Core Objects")]
    public Transform beamRayLengthController;
    private Vector3 initialRayScale;
    public Animator beamSourceAnimator;
    public Animator beamRayAnimator;
    public Animator beamEndAnimator;
    public Transform sourcePoint;
    public Transform EnemyTransform;

    [Header("Colliders")]
    public BoxCollider2D rayThinCollider;
    public BoxCollider2D rayThickCollider;
    public CapsuleCollider2D sourceThinCollider;
    public CapsuleCollider2D sourceThickCollider;
    public BoxCollider2D beamEndCollider;

    [Header("Settings")]
    public LayerMask hitMask;
    public float maxBeamDistance = 100f;
    public float timeBetweenShots = 10f;
    public float startDuration = 0.5f;
    public float activeDuration = 6f;
    public float endDuration = 0.5f;

    private Transform beamEndTransform;
    private float shotCooldownTimer = 0f;
    private bool isShooting = false;
    private bool isActivePhase;

    private void Awake()
    {
        initialRayScale = beamRayLengthController.localScale;
        beamEndTransform = beamEndAnimator.transform;
        beamSourceAnimator.transform.position = sourcePoint.position;
        beamRayLengthController.position = sourcePoint.position;
        DisableAllColliders();
        SetVisuals(false);
    }

    private void Update()
    {
        if (!isShooting)
        {
            shotCooldownTimer += Time.deltaTime;

            if (shotCooldownTimer >= timeBetweenShots)
            {
                StartCoroutine(FireBeamRoutine());
            }
        }
        else
            UpdateBeamLength();
    }

    private IEnumerator FireBeamRoutine()
    {
        isShooting = true;
        shotCooldownTimer = 0f;

        PlayPhase("starting", sourceThinCollider, rayThinCollider);
        yield return new WaitForSeconds(startDuration);

        PlayPhase("active", sourceThickCollider, rayThickCollider);
        yield return new WaitForSeconds(activeDuration);

        PlayPhase("ending", sourceThickCollider, rayThickCollider);
        yield return new WaitForSeconds(endDuration / 3f);

        SetRayCollider(rayThinCollider);
        SetSourceCollider(sourceThinCollider);
        yield return new WaitForSeconds(2f * endDuration / 3f);

        DisableAllColliders();
        ResetBeamLength();
        SetVisuals(false);

        isShooting = false;
    }

    private void PlayPhase(string phase, CapsuleCollider2D soruceCollider, BoxCollider2D rayCollider)
    {
        isActivePhase = (phase == "active");
        SetVisuals(true, false);

        beamSourceAnimator.Play(phase);
        beamRayAnimator.Play(phase);

        SetSourceCollider(soruceCollider);
        SetRayCollider(rayCollider);
        EnableBeamEndCollider(isActivePhase);

        UpdateBeamLength();
    }

    private void UpdateBeamLength()
    {
        Vector2 direction = EnemyTransform.up;
        Vector2 origin = sourcePoint.position;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxBeamDistance, hitMask);
        float distance = hit.collider != null ? hit.distance : maxBeamDistance;
        Debug.DrawRay(origin, direction * distance, Color.red);

        beamRayLengthController.localScale = new Vector3(
            initialRayScale.x * distance / 2 / EnemyTransform.localScale.x,
            initialRayScale.y,
            initialRayScale.z
        );

        beamRayLengthController.position = origin + (direction * (distance / 2));

        if (isActivePhase && hit.collider != null)
        {
            beamEndTransform.position = hit.point;
            beamEndAnimator.gameObject.SetActive(true);
        }
        else
        {
            beamEndAnimator.gameObject.SetActive(false);
        }

        ResizeCollider(distance);
    }

    private void ResizeCollider(float distance)
    {
        BoxCollider2D active = rayThinCollider.enabled ? rayThinCollider : rayThickCollider;
        active.offset = Vector2.zero;
        active.size = new Vector2(distance / beamRayLengthController.localScale.x / EnemyTransform.localScale.x / 2, active.size.y);
    }

    private void SetRayCollider(BoxCollider2D activeCollider)
    {
        rayThinCollider.enabled = activeCollider == rayThinCollider;
        rayThickCollider.enabled = activeCollider == rayThickCollider;
    }

    private void SetSourceCollider(CapsuleCollider2D activeCollider)
    {
        sourceThinCollider.enabled = activeCollider == sourceThinCollider;
        sourceThickCollider.enabled = activeCollider == sourceThickCollider;
    }

    private void EnableBeamEndCollider(bool enable = true)
    {
        if (beamEndCollider != null)
            beamEndCollider.enabled = enable;
    }

    private void DisableAllColliders()
    {
        rayThinCollider.enabled = false;
        rayThickCollider.enabled = false;
        sourceThinCollider.enabled = false;
        sourceThickCollider.enabled = false;
        if (beamEndCollider != null)
            beamEndCollider.enabled = false;
    }

    private void ResetBeamLength()
    {
        beamRayLengthController.localScale = new Vector3(
            0f,
            initialRayScale.y,
            initialRayScale.z
        );
    }

    private void SetVisuals(bool enable, bool includeBeamEnd = true)
    {
        if (beamSourceAnimator != null)
            beamSourceAnimator.gameObject.SetActive(enable);

        if (beamRayAnimator != null)
            beamRayAnimator.gameObject.SetActive(enable);

        if (beamEndAnimator != null)
            beamEndAnimator.gameObject.SetActive(enable && includeBeamEnd);
    }
}
