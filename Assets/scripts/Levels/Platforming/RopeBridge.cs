using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EdgeCollider2D))]
public class RopeBridge : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private int _numOfRopeSegments = 35;
    [SerializeField] private float _ropeSegmentLength = 0.25f;

    [Header("Physics")]
    [SerializeField] private Vector2 _gravityForce = new Vector2(0f, -2f);
    [SerializeField] private float _dampingFactor = 0.98f;

    [Header("Constraints")]
    [SerializeField] private int _numOfConstraintRuns = 50;

    [Header("Bridge")]
    public Transform StartTrans;
    public Transform EndTrans;

    [Header("Collision & Pull")]
    [SerializeField] private float _pullStrength = 25f;
    [SerializeField] private float _maxPullDistance = 15f;
    [SerializeField] private float _ropeColliderRadius = 0.1f;

    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();

    private readonly List<RopeForceSource> _activeForces = new();

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _numOfRopeSegments;

        _edgeCollider = GetComponent<EdgeCollider2D>();
        _edgeCollider.edgeRadius = _ropeColliderRadius;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Vector3 ropeStartPoint = StartTrans.position;
        for (int i = 0; i < _numOfRopeSegments; i++)
        {
            _ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= _ropeSegmentLength;
        }

        UpdateEdgeCollider();
    }

    private void Update()
    {
        DrawRope();
        UpdateEdgeCollider();
    }

    private void FixedUpdate()
    {
        Simulate();

        for (int i = 0; i < _numOfConstraintRuns; i++)
        {
            ApplyConstraints();
        }

        ApplyPersistentForces();
    }

    private void DrawRope()
    {
        Vector3[] ropePositions = new Vector3[_numOfRopeSegments];
        for (int i = 0; i < _numOfRopeSegments; i++)
        {
            ropePositions[i] = _ropeSegments[i].CurrentPosition;
        }

        _lineRenderer.SetPositions(ropePositions);
    }

    private void UpdateEdgeCollider()
    {
        Vector2[] edgePoints = new Vector2[_numOfRopeSegments];
        for (int i = 0; i < _numOfRopeSegments; i++)
        {
            edgePoints[i] = transform.InverseTransformPoint(_ropeSegments[i].CurrentPosition);
        }

        _edgeCollider.points = edgePoints;
    }

    private void Simulate()
    {
        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            Vector2 velocity = (_ropeSegments[i].CurrentPosition - _ropeSegments[i].OldPosition) * _dampingFactor;
            RopeSegment segment = _ropeSegments[i];

            segment.OldPosition = segment.CurrentPosition;
            segment.CurrentPosition += velocity;
            segment.CurrentPosition += _gravityForce * Time.fixedDeltaTime;
            _ropeSegments[i] = segment;
        }
    }

    private void ApplyConstraints()
    {
        RopeSegment firstSegment = _ropeSegments[0];
        firstSegment.CurrentPosition = StartTrans.position;
        _ropeSegments[0] = firstSegment;

        RopeSegment lastSegment = _ropeSegments[_ropeSegments.Count - 1];
        lastSegment.CurrentPosition = EndTrans.position;
        _ropeSegments[_ropeSegments.Count - 1] = lastSegment;

        for (int i = 0; i < _numOfRopeSegments - 1; i++)
        {
            RopeSegment segA = _ropeSegments[i];
            RopeSegment segB = _ropeSegments[i + 1];

            float dist = (segA.CurrentPosition - segB.CurrentPosition).magnitude;
            float error = Mathf.Abs(dist - _ropeSegmentLength);
            Vector2 changeDir = (dist > _ropeSegmentLength)
                ? (segA.CurrentPosition - segB.CurrentPosition).normalized
                : (segB.CurrentPosition - segA.CurrentPosition).normalized;

            Vector2 changeAmount = changeDir * error;

            if (i == 0)
            {
                segB.CurrentPosition += changeAmount;
                _ropeSegments[i + 1] = segB;
            }
            else if (i == _numOfRopeSegments - 2)
            {
                segA.CurrentPosition -= changeAmount;
                _ropeSegments[i] = segA;
            }
            else
            {
                segA.CurrentPosition -= changeAmount * 0.5f;
                segB.CurrentPosition += changeAmount * 0.5f;
                _ropeSegments[i] = segA;
                _ropeSegments[i + 1] = segB;
            }
        }
    }

    private void ApplyPersistentForces()
    {
        for (int i = _activeForces.Count - 1; i >= 0; i--)
        {
            RopeForceSource force = _activeForces[i];

            if (force.segmentIndex < 0 || force.segmentIndex >= _ropeSegments.Count)
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

            float pullDistance = Mathf.Min(force.velocity.magnitude, _maxPullDistance);
            Vector2 pullForce = force.velocity.normalized * pullDistance * _pullStrength * Time.fixedDeltaTime;

            RopeSegment segment = _ropeSegments[force.segmentIndex];
            segment.CurrentPosition += pullForce;
            _ropeSegments[force.segmentIndex] = segment;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        Vector2 contactPoint = collision.GetContact(0).point;

        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            float dist = Vector2.Distance(_ropeSegments[i].CurrentPosition, contactPoint);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestIndex = i;
            }
        }

        if (closestIndex != -1)
        {
            RopeForceSource existing = _activeForces.Find(f => f.segmentIndex == closestIndex);
            if (existing != null)
            {
                existing.velocity = rb.linearVelocity;
                existing.decayTimer = RopeForceSource.DecayDuration;
            }
            else
            {
                _activeForces.Add(new RopeForceSource
                {
                    segmentIndex = closestIndex,
                    velocity = rb.linearVelocity,
                    decayTimer = RopeForceSource.DecayDuration
                });
            }
        }
    }

    public struct RopeSegment
    {
        public Vector2 CurrentPosition;
        public Vector2 OldPosition;

        public RopeSegment(Vector2 pos)
        {
            CurrentPosition = pos;
            OldPosition = pos;
        }
    }

    private class RopeForceSource
    {
        public int segmentIndex;
        public Vector2 velocity;
        public float decayTimer;
        public const float DecayDuration = 0.3f;
    }
}
