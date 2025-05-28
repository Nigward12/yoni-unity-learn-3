using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

public class RopeSegment
{
    public Vector2 CurrentPosition;
    public Vector2 PreviousPosition;

    public RopeSegment(Vector2 pos)
    {
        CurrentPosition = pos;
        PreviousPosition = pos;
    }
}
public class RopeVerlet : MonoBehaviour
{
    // EITHER STAY WITH THE COLLIDER THING OR MOVE TO THE PROBE
    // THING GPT SUGGESTED, ALSO MAYBE SPLIT THE BRIDGE TO A DIFFERENT SCRIPT
    // IF NOT SPLIT, HANDLE THE EDITOR LOGIC OF WHAT SHOUD BE VISIBLE AND WHAT NOT 
    // UNDER CERTAIN CODITIONS
    // THE EDGE COLLIDER THING MIGHT WORK IF I CAN MAKE SURE IT APPLYS POWER EVERY FRAME
    // WHEN SOMETHING IS COLLIDING WITH IT
    [Header("Rope vars")]
    [SerializeField] private int ropeSegmentsNumber = 50;
    [SerializeField] private float ropeSegmentLength = 0.225f;
    [SerializeField] private bool RopeDangle = false;

    [Header("Physics")]
    [SerializeField] private Vector2 gravityForce = new Vector2(0f, -2f);
    [SerializeField] private float dampingFactor = 0.98f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionRadius = 0.1f;
    [SerializeField] private float bounceFactor = 0.1f;
    [SerializeField] private float correctionClampAmount = 0.1f;

    [Header("Constraints")]
    [SerializeField] private int numOfConstraintRuns = 50;

    [Header("Bridge")]
    [SerializeField] private bool isBridge = false;
    [SerializeField] private Transform StartTrans;
    [SerializeField] private Transform EndTrans;
    [SerializeField] private float bridgePullFactor = 300f;
    [SerializeField] private float bridgeMaxPullDistance = 15f;
    [SerializeField] private float ropeColliderRadius = 0.1f;
    //make it so bridge has to be elastic maybe, make bridge and elastic
    // properties only appear after setting it as true

    [Header("Optimizations")]
    public int collisionSegmentInterval = 2;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private LineRenderer ropeLineRenderer;
    private Vector3 ropeStartPoint;
    private EdgeCollider2D ropeCollider;
    private Vector3[] ropePositions;

    private void Awake()
    {
        ropeLineRenderer = GetComponent<LineRenderer>();
        ropeLineRenderer.positionCount = ropeSegmentsNumber;

        if (RopeDangle)
            ropeStartPoint = transform.position;
        else if (isBridge)
            ropeStartPoint = StartTrans.position;
        else
            ropeStartPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        for (int i = 0; i < ropeSegmentsNumber; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegmentLength;
        }

        if (isBridge)
        {
            ropeCollider = gameObject.AddComponent<EdgeCollider2D>();
            ropeCollider.edgeRadius = ropeColliderRadius;
            ropeCollider.points = new Vector2[ropeSegmentsNumber];
            ropeCollider.isTrigger = false;
        }
        ropePositions = new Vector3[ropeSegmentsNumber];
    }

    private void Update()
    {
        DrawRope();
        if (isBridge)
            UpdateEdgeCollider();
    }

    private void FixedUpdate()
    {
        SimulateRopePhysics();

        for (int i = 0;i < numOfConstraintRuns;i++)
        {
            ApplySegmentsConstraints();
            if (!isBridge && i % collisionSegmentInterval == 0)
                HandleCollisions();
        }
    }

    private void DrawRope()
    {
        for (int i = 0; i < ropeSegmentsNumber; i++)
            ropePositions[i] = ropeSegments[i].CurrentPosition;

        ropeLineRenderer.SetPositions(ropePositions);
    }

    private void UpdateEdgeCollider()
    {
        Vector2[] edgePoints = new Vector2[ropeSegmentsNumber];
        for (int i = 0; i < ropeSegmentsNumber; i++)
            edgePoints[i] = transform.InverseTransformPoint(ropeSegments[i].CurrentPosition);

        ropeCollider.points = edgePoints;
    }

    private void SimulateRopePhysics()
    {
        foreach (var ropeSegment in ropeSegments)
        {
            Vector2 newVelocity = (ropeSegment.CurrentPosition - ropeSegment.PreviousPosition) * dampingFactor;

            ropeSegment.PreviousPosition = ropeSegment.CurrentPosition;
            ropeSegment.CurrentPosition += newVelocity;
            ropeSegment.CurrentPosition += gravityForce * Time.fixedDeltaTime;
        }
    }

    private void ApplySegmentsConstraints()
    {
        if (RopeDangle)
            ropeSegments[0].CurrentPosition = transform.position;
        else if (isBridge)
        {
            ropeSegments[0].CurrentPosition = StartTrans.position;
            ropeSegments[ropeSegmentsNumber - 1].CurrentPosition = EndTrans.position;
        }
        else
            ropeSegments[0].CurrentPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        for (int i = 0; i< ropeSegmentsNumber -1; i++)
        {
            RopeSegment currentSeg = ropeSegments[i];
            RopeSegment nextSeg = ropeSegments[i + 1];

            Vector2 vectorDist = currentSeg.CurrentPosition - nextSeg.CurrentPosition;
            float dist = vectorDist.magnitude;
            //difference - how far of is the current distance between the segments from the distance that should be
            float difference = dist - ropeSegmentLength;

            Vector2 distanceChangeDir = vectorDist.normalized;
            Vector2 changeVector = distanceChangeDir * difference;

            if (i == 0)
                nextSeg.CurrentPosition += changeVector;
            else if (isBridge && i == ropeSegmentsNumber - 2)
                currentSeg.CurrentPosition -= changeVector;
            else
            {
                currentSeg.CurrentPosition -= (changeVector * 0.5f);
                nextSeg.CurrentPosition += (changeVector * 0.5f);
            }

            ropeSegments[i] = currentSeg;
            ropeSegments[i+1] = nextSeg;    
        }
    }

    private void HandleCollisions()
    {
        // Skip the first segment (attached to the mouse) - for now
        for (int i = 1; i < ropeSegments.Count; i++)
        {
            RopeSegment segment = ropeSegments[i];
            Vector2 velocity = segment.CurrentPosition - segment.PreviousPosition;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(segment.CurrentPosition, collisionRadius, collisionMask);

            foreach (Collider2D collider in colliders)
            {
                Vector2 closestPoint = collider.ClosestPoint(segment.CurrentPosition);
                float distance = Vector2.Distance(segment.CurrentPosition, closestPoint);

                if (distance < collisionRadius)
                {
                    Vector2 normal = (segment.CurrentPosition - closestPoint).normalized;
                    if (normal == Vector2.zero)
                        normal = (segment.CurrentPosition - (Vector2)collider.transform.position).normalized;

                    float CollisionRadiusPenetration = collisionRadius - distance;
                    // Clamp correction to avoid large jumps
                    //float correction = Mathf.Min(CollisionRadiusPenetration, correctionClampAmount);
                    //segment.CurrentPosition += normal * correction;
                    segment.CurrentPosition += normal * CollisionRadiusPenetration;
                    velocity = Vector2.Reflect(velocity, normal) * bounceFactor;
                }
            }

            segment.PreviousPosition = segment.CurrentPosition - velocity;
            ropeSegments[i] = segment;
        }
    }

    public void ApplyExternalForce(int index, Vector2 externalVelocity)
    {
        if (index < 0 || index >= ropeSegments.Count) return;

        print("here");
        RopeSegment segment = ropeSegments[index];

        // Use object's velocity and scale it for rope pull effect
        Vector2 pullForce = externalVelocity * bridgePullFactor * Time.fixedDeltaTime;

        segment.CurrentPosition += pullForce;
        ropeSegments[index] = segment;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var rb = collision.rigidbody;
        if (rb != null)
        {
            Vector2 contactPoint = collision.GetContact(0).point;

            float minDistance = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < ropeSegments.Count; i++)
            {
                float dist = Vector2.Distance(ropeSegments[i].CurrentPosition, contactPoint);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                ApplyExternalForce(closestIndex, rb.linearVelocity);
            }
        }
    }
}

