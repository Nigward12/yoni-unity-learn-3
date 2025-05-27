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
    [Header("Rope vars")]
    [SerializeField] private int ropeSegmentsNumber = 50;
    [SerializeField] private float ropeSegmentLength = 0.225f;

    [Header("Physics")]
    [SerializeField] private Vector2 gravityForce = new Vector2(0f, -2f);
    [SerializeField] private float dampingFactor = 0.98f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionRadius = 0.1f;
    [SerializeField] private float bounceFactor = 0.1f;
    [SerializeField] private float correctionClampAmount = 0.1f;

    [Header("Constraints")]
    [SerializeField] private int numOfConstraintRuns = 50;

    [Header("Optimizations")]
    public int _collisionSegmentInterval = 2;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private LineRenderer ropeLineRenderer;
    private Vector3 ropeStartPoint;

    private void Awake()
    {
        ropeLineRenderer = GetComponent<LineRenderer>();
        ropeLineRenderer.positionCount = ropeSegmentsNumber;

        ropeStartPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        for (int i = 0; i < ropeSegmentsNumber; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegmentLength;
        }
    }

    private void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        SimulateRopePhysics();
        
        for (int i = 0;i < numOfConstraintRuns;i++)
        {
            ApplySegmentsConstraints();
            if (i % _collisionSegmentInterval == 0)
                HandleCollisions();
        }
    }

    private void DrawRope()
    {
        ropeLineRenderer.SetPositions(ropeSegments.Select(s => (Vector3)s.CurrentPosition).ToArray());
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

            if (i != 0)
            {
                currentSeg.CurrentPosition -= (changeVector * 0.5f);
                nextSeg.CurrentPosition += (changeVector * 0.5f);
            }
            else
                nextSeg.CurrentPosition += changeVector;

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
                    //float correction = Mathf.Min(penetration, _correctionClampAmount);
                    //segment.CurrentPosition += normal * correction;
                    segment.CurrentPosition += normal * CollisionRadiusPenetration;

                    velocity = Vector2.Reflect(velocity, normal) * bounceFactor;
                }
            }

            segment.PreviousPosition = segment.CurrentPosition - velocity;
            ropeSegments[i] = segment;
        }
    }
}
