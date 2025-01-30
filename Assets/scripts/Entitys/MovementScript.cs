using UnityEngine;

public class MovementScript : MonoBehaviour
{
    [SerializeField] private bool facingLeftOnStart;
    [SerializeField] protected float defaultGravityScale;
    [SerializeField] protected float defaultLocalScale;
    protected int leftScaleDirection;
    protected bool facingLeft;
    protected virtual void Awake()
    {
        if (facingLeftOnStart)
            leftScaleDirection = (int)Mathf.Sign(transform.localScale.x) * 1;
        else
            leftScaleDirection = (int)Mathf.Sign(transform.localScale.x) * -1;
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
