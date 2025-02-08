using UnityEngine;

public abstract class MovementScript : MonoBehaviour
{
    [SerializeField] private bool facingLeftOnStart;
    [SerializeField] protected float defaultGravityScale;
    [SerializeField] protected float defaultLocalScale;
    [SerializeField] protected PhysicsMaterial2D noFriction;
    [SerializeField] protected PhysicsMaterial2D fullFriction;
    protected int leftScaleDirection;
    protected bool facingLeft;
    protected Rigidbody2D body;
    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        if (facingLeftOnStart)
            leftScaleDirection = (int)Mathf.Sign(transform.localScale.x) * 1;
        else
            leftScaleDirection = (int)Mathf.Sign(transform.localScale.x) * -1;
    }

    protected void stopMovementOnDeath()
    {
        body.sharedMaterial = fullFriction;
    }

    protected virtual void Update()
    {
        facingLeft = (int)Mathf.Sign(transform.localScale.x) * 1 == leftScaleDirection;
    }

    protected void TurnOnXAxis()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,
            transform.localScale.z);
    }

    public bool isFacingLeft()
    {
        return facingLeft;
    }

    public int getLeftScaleDirection()
    {
        return leftScaleDirection;
    }


}
