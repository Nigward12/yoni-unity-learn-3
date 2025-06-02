using UnityEngine;

public class RopeSegmentTrigger : MonoBehaviour
{
    public int segmentIndex { get; private set; }
    public RopeBridgeProbes bridge { get; private set; }

    public void SetBridge(RopeBridgeProbes bridge)
        { this.bridge = bridge; }
    public void SetSegmentIndex(int segmentIndex)
        { this.segmentIndex = segmentIndex; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
            bridge.AddForceToSegment(segmentIndex, rb);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
            bridge.AddForceToSegment(segmentIndex, rb);
    }

}
