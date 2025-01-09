using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerBasicMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float walljumpPowerX;
    [SerializeField] private float walljumpPowerY;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private BoxCollider2D feetCollider;
    [SerializeField] private float defaultGravityScale;
    [SerializeField] private float defaultLocalScale;
    [SerializeField] private float maxWalkableHeightDifference;
    private bool onSlope; 
    private Vector2 slopeNormal; 
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D bodyCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bodyCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipPlayerLeftRight();

        SetAnimatorParams();

        if(WallJumpReady())
        {
            UpdateMovement();
            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
    }

    #region updateMethods

    private void FlipPlayerLeftRight()
    {
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(defaultLocalScale,defaultLocalScale,defaultLocalScale);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-defaultLocalScale,
                defaultLocalScale, defaultLocalScale);
    }

    private void SetAnimatorParams()
    {
        bool isGrounded = IsGrounded();
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
        if (!isGrounded && !OnWall() && body.linearVelocityY < 0 && !onSlope)
        {
            anim.SetTrigger("fall");
            body.gravityScale = defaultGravityScale * 1.5f;
        }
        else
            body.gravityScale = defaultGravityScale;
    }

    private void UpdateMovement()
    {
        if (onSlope)
        {
            if (onSlope)
            {
                Vector2 slopeDirection = new Vector2(slopeNormal.x, slopeNormal.y).normalized;

                if (horizontalInput < 0)
                    slopeDirection.x = -Mathf.Abs(slopeDirection.x);

                body.linearVelocity = slopeDirection * Mathf.Abs(horizontalInput) * speed;
            }
        }
        else
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocityY);
        }
    }

    private bool WallJumpReady()
    {
        //return true if the a wall jump can be performed, also configures the players 
        // body accordingly
        if (wallJumpCooldown > 0.2f)
        {

            if (OnWall() && !IsGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
                body.gravityScale = defaultGravityScale;
            return true;
        }
        else
            wallJumpCooldown += Time.deltaTime;
        return false;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            RegularJump();
        }
        else if (OnWall())
        {
            WallJump();
        }
    }

    private void RegularJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocityX, jumpPower);
        anim.SetTrigger("jump");
    }

    private void WallJump()
    {
        if (horizontalInput == 0)
        {
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * walljumpPowerX + walljumpPowerY, 0);
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * walljumpPowerX, walljumpPowerY);

        wallJumpCooldown = 0;
    }


    #endregion

    #region playerStateIndicators
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(feetCollider.bounds.center,
            feetCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        if (raycastHit.collider != null)
        {
            slopeNormal = raycastHit.normal;
            float slopeAngle = Vector2.Angle(Vector2.up, slopeNormal);

            onSlope = slopeAngle > 0;
        }
        else
            onSlope = false;

        float direction = Mathf.Sign(transform.localScale.x);

        Vector2 rayOrigin = new Vector2(transform.position.x + direction * 0.5f, transform.position.y); 
        RaycastHit2D raycastHitForward = Physics2D.Raycast(rayOrigin, Vector2.down, 1.5f, groundLayer); 

        if (raycastHit.collider != null)
        {
            float heightDifference = Mathf.Abs(raycastHit.point.y - transform.position.y);
            print(heightDifference);
            if (heightDifference <= maxWalkableHeightDifference)
            {
                return true; 
            }
        }

        return false;
    }
    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bodyCollider.bounds.center,
            bodyCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
    public bool CanAttack()
    {
        return horizontalInput == 0 && IsGrounded() && !OnWall();
    }
    #endregion
}