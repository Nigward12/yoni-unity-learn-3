
using UnityEngine;
using System.Linq;

public class PlayerBasicMovement : MovementScript
{
    [Header("Movement variables")]
    [SerializeField] private float speed;
    [SerializeField] private LayerMask wallLayer;
    private float leftScale;
    [SerializeField] private float maxFallSpeed;

    [Header("Jump variables")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float walljumpForceX;
    [SerializeField] private float walljumpForceY;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private int extraJumpsAvailable;
    private int extraJumpsCurrent;
    private float jumpBufferTimer;
    //private float lastJumpTime;
    private bool jumping;
    private float coyoteTimer = Mathf.Infinity;

    [Header ("GroundAndSlopeCheck")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private BoxCollider2D feetCollider;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
    private bool isGrounded, onWall;
    private RaycastHit2D underFeetRaycastHit;

    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float wallAngle;
    private bool onSlope;
    private Vector2 slopeNormal;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private bool canWalkOnSlope;

    [Header("SFX")]
    [SerializeField] private Sound jumpSound;
    [SerializeField] private Sound jumpLandingSound;
    [SerializeField] private Sound runningSound;
    private bool landingSoundPlayed;
    private AudioSource runningAudioSource;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D bodyCollider;
    //private float wallJumpCooldown;
    private float horizontalInput;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bodyCollider = GetComponent<BoxCollider2D>();
        leftScale = defaultLocalScale * leftScaleDirection;
        extraJumpsCurrent = extraJumpsAvailable;
    }

    protected override void Update()
    {
        base.Update();

        horizontalInput = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        onWall = OnWall();

        FlipPlayerLeftRight();

        SetAnimatorParams();

        UpdateStateSounds();

        SlopeCheck();

        UpdateJumpState();

        UpdateWallJumpState();

        UpdateMovement();

        UpdateJumps();

    }

    #region updateMethods

    private void FlipPlayerLeftRight()
    {
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(-leftScale, defaultLocalScale, defaultLocalScale);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(leftScale, defaultLocalScale, defaultLocalScale);
    }

    private void SetAnimatorParams()
    {
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
        if (Falling())
        {
            anim.SetBool("falling", true);
            body.gravityScale = defaultGravityScale * 1.5f;
        }
        else
        {
            anim.SetBool("falling", false);
            body.gravityScale = defaultGravityScale;
        }
    }

    private void UpdateStateSounds()
    {
        UpdateStateSound(ref runningAudioSource, runningSound,
            new bool[] { isGrounded, horizontalInput != 0 });
    }

    private void UpdateStateSound(ref AudioSource stateAudioSource, Sound stateSound, bool[] conditions)
    {
        if (conditions.All(condition => condition))
        {
            if (stateAudioSource == null)
            {
                stateAudioSource = SoundManager.instance.PlayLoopingSound(stateSound);
            }
        }
        else
        {
            if (stateAudioSource != null)
            {
                Destroy(stateAudioSource.gameObject);
                stateAudioSource = null;
            }
        }
    }

    private void UpdateMovement()
    {
        if (isGrounded && !onSlope)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, 0f);
        }
        else if (isGrounded && onSlope && canWalkOnSlope)
        {
            body.linearVelocity = new Vector2(speed * slopeNormal.x * -horizontalInput,
                speed * slopeNormal.y * -horizontalInput);
        }
        else if (!isGrounded)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed,
                Mathf.Clamp(body.linearVelocity.y, -maxFallSpeed, float.MaxValue));
        }
    }

    private void UpdateJumps()
    {
        if (Input.GetKeyDown(KeyCode.Space)/* || Time.time - lastJumpTime > jumpBufferTime*/)
            Jump(true);

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocityY > 0)
            body.linearVelocity = new Vector2(body.linearVelocityX, body.linearVelocityY / 2);
    }

    private void UpdateWallJumpState()
    {
        if (onWall)
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else
         body.gravityScale = defaultGravityScale;
    }

    //private bool WallJumpReady()
    //{
    //    //return true if the a wall jump can be performed, also configures the players 
    //    // body accordingly
    //    if (wallJumpCooldown > 0.2f)
    //    {

    //        if (OnWall() && !isGrounded)
    //        {
    //            body.gravityScale = 0;
    //            body.linearVelocity = Vector2.zero;
    //        }
    //        else
    //            body.gravityScale = defaultGravityScale;
    //        return true;
    //    }
    //    else
    //        wallJumpCooldown += Time.deltaTime;
    //    return false;
    //}

    private void Jump(bool playSound)
    {
        if ( coyoteTimer <= 0 && !onWall && extraJumpsCurrent <= 0)
            return;


        if (onWall)
        {
            jumping = true;

            WallJump();

            coyoteTimer = 0;

            //lastJumpTime = Time.time;
        }
        else 
        {
            if (playSound)
                SoundManager.instance.PlaySound(jumpSound);

            jumping = true;

            RegularJump();

            //lastJumpTime = Time.time;

            if (coyoteTimer <= 0)
                extraJumpsCurrent--;

            coyoteTimer = 0;

            jumpBufferTimer = jumpBufferTime;
        }
    }

    private void UpdateJumpState()
    {
        if (jumping)
        {
            if (isGrounded || onWall)
            {
                jumping = false;
                landingSoundPlayed = false;
            }
            else if (almostGrounded() && body.linearVelocityY < 0
                && !landingSoundPlayed)
            {
                SoundManager.instance.PlaySound(jumpLandingSound);
                landingSoundPlayed = true;
            }
        }
        if (!onWall)
        {
            if (isGrounded)
            {
                coyoteTimer = coyoteTime;
                extraJumpsCurrent = extraJumpsAvailable;
            }
            else
                coyoteTimer -= Time.deltaTime;
        }
    }

    private void stopMovementOnDeath()
    {
        body.sharedMaterial = fullFriction;
    }    

    private void RegularJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocityX, jumpPower);
        anim.SetTrigger("jump");
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * walljumpForceX,
            walljumpForceY));
    }


    #endregion

    #region playerStateIndicators

    private bool almostGrounded()
    {
        underFeetRaycastHit = Physics2D.BoxCast(feetCollider.bounds.center,
            feetCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);
        return underFeetRaycastHit.collider != null;
    }
    private bool IsGrounded()
    {
        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
            return false;
        }
        underFeetRaycastHit = Physics2D.BoxCast(feetCollider.bounds.center,
            feetCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return underFeetRaycastHit.collider != null;
        
    }

    private void SlopeCheck()
    {
        SlopeCheckVertical();
        SlopeCheckHorizontal();
    }

    private void SlopeCheckVertical()
    {
        underFeetRaycastHit = Physics2D.Raycast(feetCollider.bounds.center, Vector2.down, slopeCheckDistance, groundLayer);

        if (underFeetRaycastHit)
        {
            slopeNormal = Vector2.Perpendicular(underFeetRaycastHit.normal).normalized;

            slopeDownAngle = Vector2.Angle(underFeetRaycastHit.normal, Vector2.up);

            if (slopeDownAngle != 0 || slopeSideAngle != 0)
                onSlope = true;
            else
                onSlope = false;


            //Debug.DrawRay(underFeetRaycastHit.point, slopeNormal, Color.red);
            //Debug.DrawRay(underFeetRaycastHit.point, underFeetRaycastHit.normal, Color.green);
        }

        canWalkOnSlope = slopeDownAngle <= maxSlopeAngle && slopeSideAngle <= maxSlopeAngle;

        //print(onSlope + ", canwalk: " + canWalkOnSlope + ", horizontalinput: " + horizontalInput + ", slope side:" + slopeSideAngle);

        if (onSlope && horizontalInput == 0f && canWalkOnSlope)
            body.sharedMaterial = fullFriction;
        else
            body.sharedMaterial = noFriction;
    }

    private void SlopeCheckHorizontal()
    {
        Vector2 rayDirectionFront = transform.right * (slopeCheckDistance * 0.75f);
        Vector2 rayDirectionBack = -transform.right * (slopeCheckDistance * 0.75f);

        RaycastHit2D slopeHitFront = Physics2D.Raycast(feetCollider.bounds.center,
            transform.right, (slopeCheckDistance * 0.75f), groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(feetCollider.bounds.center,
            -transform.right, (slopeCheckDistance * 0.75f), groundLayer);

        //Debug.DrawRay(feetCollider.bounds.center, rayDirectionFront, Color.cyan);
        //Debug.DrawRay(feetCollider.bounds.center, rayDirectionBack, Color.cyan);
        if (slopeHitFront)
        {
            if (Vector2.Angle(slopeHitFront.normal, Vector2.up) < wallAngle)
            {
                onSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            }
        }
        else if (slopeHitBack)
        {
            if (Vector2.Angle(slopeHitFront.normal, Vector2.up) < wallAngle)
            {
                onSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            }
        }
        else
        {
            slopeSideAngle = 0f;
            onSlope = false;
        }
    }

    
    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bodyCollider.bounds.center,
            bodyCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null && !isGrounded;
    }
    public bool CanAttack()
    {
        return horizontalInput == 0 && isGrounded && !OnWall();
    }

    private bool Falling()
    {
        underFeetRaycastHit = Physics2D.BoxCast(feetCollider.bounds.center,
            feetCollider.bounds.size, 0, Vector2.down, slopeCheckDistance, groundLayer);
        if (!isGrounded && !OnWall() && body.linearVelocityY < 0 && !underFeetRaycastHit.collider)
           return true;
        return false;
    }
    #endregion
}