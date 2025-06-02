using UnityEngine;
using System.Collections.Generic;
using static RopeBridge;

[RequireComponent(typeof(LineRenderer))]
public class RopeBridgeProbes : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private int ropeSegmentsNum = 50;
    [SerializeField] private float ropeSegmentLength = 0.25f;

    [Header("Physics")]
    [SerializeField] private Vector2 gravityForce = new Vector2(0f, -2f);
    [SerializeField] private float dampingFactor = 0.98f;
    [SerializeField] private float edgeColliderRadius = 0.1f;
    [SerializeField] private LayerMask collisionMask;

    [Header("Constraints")]
    [SerializeField] private int numOfConstraintRuns = 50;

    [Header("Bridge")]
    public Transform StartTrans;
    public Transform EndTrans;

    [Header("Momentum")]
    [SerializeField] private float momentumFactor = 1f;
    [SerializeField] private float maxStretchDistance = 5f;

    [Header("Probes")]
    [SerializeField] private GameObject collisionProbePrefab;

    private EdgeCollider2D _edgeCollider;
    private LineRenderer _lineRenderer;
    private List<RopeSegment> ropeSegments = new();
    private GameObject[] _segmentProbes;
    private readonly List<RopeForceSource> _activeForces = new();

    private void Awake()
    {
        // MAYBE TURN THE PROBES INTO ACTUAL COLLIDERS INSTEAD OF USING AN EDGE COLLIDER
        // IF SO MAYBE CONSIDER WHAT GPT SAID ABOUT USING ITS NORMAL TO DETECT IF THE COLLISION
        // WAS FROM ABOVE THE ROPE
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = ropeSegmentsNum;

        _segmentProbes = new GameObject[ropeSegmentsNum];

        Vector3 ropeStartPoint = StartTrans.position;
        for (int i = 0; i < ropeSegmentsNum; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegmentLength;

            GameObject probe = Instantiate(collisionProbePrefab, ropeSegments[i].CurrentPosition, Quaternion.identity, transform);
            probe.GetComponent<RopeSegmentTrigger>().SetSegmentIndex(i);
            probe.GetComponent<RopeSegmentTrigger>().SetBridge(this);
            _segmentProbes[i] = probe;
        }

        _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        _edgeCollider.edgeRadius = edgeColliderRadius;
        _edgeCollider.isTrigger = false;
    }

    private void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        SimulateRopePhysics();
        UpdateEdgeCollider();
        for (int i = 0; i < numOfConstraintRuns; i++)
        {
            ApplyConstraints();
            ApplyPersistentForces();
        }

    }

    private void UpdateEdgeCollider()
    {
        Vector2[] edgePoints = new Vector2[ropeSegmentsNum];
        for (int i = 0; i < ropeSegmentsNum; i++)
            edgePoints[i] = transform.InverseTransformPoint(ropeSegments[i].CurrentPosition);

        _edgeCollider.points = edgePoints;
    }

    private void DrawRope()
    {
        Vector3[] ropePositions = new Vector3[ropeSegmentsNum];
        for (int i = 0; i < ropeSegmentsNum; i++)
        {
            ropePositions[i] = ropeSegments[i].CurrentPosition;
            if (_segmentProbes[i] != null)
            {
                _segmentProbes[i].transform.position = ropeSegments[i].CurrentPosition;
            }
        }

        _lineRenderer.SetPositions(ropePositions);
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

    private void ApplyConstraints()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.CurrentPosition = StartTrans.position;
        ropeSegments[0] = firstSegment;

        RopeSegment lastSegment = ropeSegments[ropeSegments.Count - 1];
        lastSegment.CurrentPosition = EndTrans.position;
        ropeSegments[ropeSegments.Count - 1] = lastSegment;

        for (int i = 0; i < ropeSegmentsNum - 1; i++)
        {
            RopeSegment segA = ropeSegments[i];
            RopeSegment segB = ropeSegments[i + 1];

            float dist = (segA.CurrentPosition - segB.CurrentPosition).magnitude;
            float error = Mathf.Abs(dist - ropeSegmentLength);
            Vector2 changeDir = (dist > ropeSegmentLength)
                ? (segA.CurrentPosition - segB.CurrentPosition).normalized
                : (segB.CurrentPosition - segA.CurrentPosition).normalized;

            Vector2 changeAmount = changeDir * error;

            if (i == 0)
            {
                segB.CurrentPosition += changeAmount;
                ropeSegments[i + 1] = segB;
            }
            else if (i == ropeSegmentsNum - 2)
            {
                segA.CurrentPosition -= changeAmount;
                ropeSegments[i] = segA;
            }
            else
            {
                segA.CurrentPosition -= changeAmount * 0.5f;
                segB.CurrentPosition += changeAmount * 0.5f;
                ropeSegments[i] = segA;
                ropeSegments[i + 1] = segB;
            }
        }
    }


    private void ApplyPersistentForces()
    {
        for (int i = _activeForces.Count - 1; i >= 0; i--)
        {
            RopeForceSource force = _activeForces[i];

            if (force.segmentIndex < 0 || force.segmentIndex >= ropeSegments.Count || force.rb == null)
            {
                _activeForces.RemoveAt(i);
                continue;
            }

            force.decayTimer -= Time.fixedDeltaTime;
            if (force.decayTimer <= 0f)
            {
                _activeForces.RemoveAt(i);
                continue;
            }

            RopeSegment segment = ropeSegments[force.segmentIndex];
            if (!force.hasAppliedInitialImpact)
            {
                Vector2 velocity = force.rb.linearVelocity;
                Vector2 impulse = velocity.normalized * Mathf.Min(velocity.magnitude, maxStretchDistance) * momentumFactor * Time.fixedDeltaTime;
                Vector2 proposedPosition = segment.CurrentPosition + impulse;
                Vector2 delta = proposedPosition - segment.CurrentPosition;

                if (!Physics2D.CircleCast(segment.CurrentPosition, edgeColliderRadius, delta.normalized, delta.magnitude, collisionMask))
                    segment.CurrentPosition = proposedPosition;

                force.hasAppliedInitialImpact = true;
            }
            else if (force.rb.transform.position.y > ropeSegments[force.segmentIndex].CurrentPosition.y)
            {
                // check that standing below a rope also consistently stretches it up a little if
                // the player is very close to the rope
                float gravityForceScalar = -gravityForce.y;
                Vector2 weightForce = Vector2.down * gravityForceScalar * force.rb.mass * momentumFactor * Time.fixedDeltaTime;
                segment.CurrentPosition += weightForce;
            }

            ropeSegments[force.segmentIndex] = segment;
        }
    }


    public void AddForceToSegment(int segmentIndex, Rigidbody2D rb)
    {
        RopeForceSource existing = _activeForces.Find(f => f.segmentIndex == segmentIndex && f.rb == rb);
        if (existing != null)
        {
            existing.decayTimer = RopeForceSource.DecayDuration;
        }
        else
        {
            _activeForces.Add(new RopeForceSource
            {
                segmentIndex = segmentIndex,
                rb = rb,
                decayTimer = RopeForceSource.DecayDuration
            });
        }
    }


    private class RopeForceSource
    {
        public int segmentIndex;
        public Rigidbody2D rb;
        public Vector2 velocity;
        public float decayTimer;
        public bool hasAppliedInitialImpact = false;
        public const float DecayDuration = 0.2f;
    }
}