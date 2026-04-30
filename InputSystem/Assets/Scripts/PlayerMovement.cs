using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // This part holds variables that are used in a code or can be changed in unity any time 
    // For example we can change move speed, jump power and so on 

    public Rigidbody2D rb;
    public Animator animator;
    public CapsuleCollider2D playerCollider;
    bool isFacingRight = true;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallMovement")]
    public Vector2 wallJumpPower = new Vector2(5f, 10f);
    bool isWallGrabbing;
    float wallJumpDirection;
    public float wallJumpTime = 0.2f;
    private float wallJumpCounter;

    [Header("Sliding")]
    public float slideSpeed = 12f;
    public float slideDrag = 5f;
    public float slideHeight = 0.5f;
    public float slideCooldown = 1f;
    private float slideCooldownCounter;
    private float originalColliderHeight;
    private Vector2 originalColliderOffset;
    private bool isSliding;
    private float currentSlideSpeed;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;



    [Header("Audio")]
    public AudioSource sfxSource;

    [Header("Footstep Sounds")]
    public AudioClip PlayerWalkingCave;
    public AudioClip PlayerWalkingGrass;

    [Header("Landing Sounds")]
    public AudioClip PlayerLandingCave;
    public AudioClip PlayerLandingGrass;

    [Header("Wall Sounds")]
    public AudioClip PlayerWallHold;
    public AudioClip PlayerWallHop;

    private bool isWalkingSoundPlaying = false;
    private bool wasGrounded = false;
    private bool wallHoldPlayed = false;


    // Just holds player position and collider 
    void Start()
    {
        if (playerCollider != null)
        {
            originalColliderHeight = playerCollider.size.y;
            originalColliderOffset = playerCollider.offset;
        }
    }

    // Updates every frame 
    void Update()
    {
        // Every time we check is the player is on the ground 
        // Checks the gravity 
        // Checks if player is grabbing the wall 
        GroundCheck();
        Gravity();
        WallGrab();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // While we sliding, we cant slide again 
        if (slideCooldownCounter > 0)
        {
            slideCooldownCounter -= Time.deltaTime;
        }

        // Player cant change direction for a short period after wall jump 
        if (wallJumpCounter > 0)
        {
            wallJumpCounter -= Time.deltaTime;
        }

        // While player is not holding onto the wall, player can slide or simply move 
        else if (!isWallGrabbing)
        {
            if (isSliding)
            {
                PerformSlide();
            }
            else
            {
                rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
                Flip();
            }
        }

        HandleWalkingSound();
        wasGrounded = isGrounded;

        HandleWalkingSound();
        wasGrounded = isGrounded;

        float moveSpeedForAnim = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", moveSpeedForAnim);

        animator.SetBool("isGrounded", isGrounded);

        animator.SetBool("isWallGrabbing", isWallGrabbing);
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("isSliding", isSliding);

    }

    // Lets player move left or right 
    // The inputs so far are WASD, arrow keys 
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // Controls all the jumps, from simple ground jump, double jump to wall jump. 
    public void Jump(InputAction.CallbackContext context)
    {
        // If the button to jump is pressed 
        if (context.performed)
        {
            // If we sliding and press jump, we stop sliding 
            if (isSliding)
            {
                StopSlide();
            }


            // Controls how many jumps we have, if we double jump from ground to a wall, jumping away from the wall we only have 1 jump and the other way. 
            if (coyoteTimeCounter < 0 && jumpsRemaining == maxJumps && !isWallGrabbing)
            {
                jumpsRemaining--;
            }

            // Wall jump 
            if (isWallGrabbing)
            {
                sfxSource.PlayOneShot(PlayerWallHop);

                // When we on the wall and press jump, we instantly get off the wall 
                isWallGrabbing = false;

                // This disables the movement control for a very short time when we jump off the wall we cant control the character. 
                wallJumpCounter = wallJumpTime;
                // Pushes away a character X and Y power away from the wall 
                rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);


                // Makes sure that usually we only have 1 jump available when we jump from the wall. (0 no jumps to 1 max jump) 
                jumpsRemaining = Mathf.Clamp(jumpsRemaining, 0, 1);

                // Flips character to the side we jump when we jump off the wall. 


                if ((wallJumpDirection > 0 && !isFacingRight) || (wallJumpDirection < 0 && isFacingRight))
                {
                    FlipDirectly();
                }
                return;
            }

            // Simple and double jump 
            if (jumpsRemaining > 0 && (isGrounded || coyoteTimeCounter > 0 || jumpsRemaining < maxJumps))
            {
                if (!isGrounded)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                }

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                coyoteTimeCounter = 0;
            }
        }

        // This part controls the power of the jump 
        // If we hold down the jump button fully we jump higher 
        // If we press jump button and quickly release it, the jump will be smaller 
        if (context.canceled)
        {
            // Checks if the player is moving up 
            if (rb.linearVelocity.y > 0)
            {
                // Multiply current Y velocity to cut the jump short (0.5f = half power)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    // Button that controls sliding, at the moment sliding works on SHIFT, but it can be changed. 
    public void OnShift(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !isSliding && slideCooldownCounter <= 0)
        {
            StartSlide();
        }
    }

    // Stars sliding 
    private void StartSlide()
    {
        // Tells the code that player is sliding 
        // Give a speed that launches the player forwards 
        // We can give a timer so the player cant constantly spam slide 
        isSliding = true;
        currentSlideSpeed = slideSpeed;
        slideCooldownCounter = slideCooldown;

        // Cuts player collider scale as we dash 
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        
        // Tough line, also cuts off collider 
        float offsetDifference = (originalColliderHeight - slideHeight) / 2f;
        playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - offsetDifference);
    }

    // Controls the whole math behind sliding 
    private void PerformSlide()
    {
        // The direction player is facing 
        float direction;

        // Checks which way the character is facing, this is a must so we dont slide backwards. 
        // We need it so we slide the same way we looking at. 
        if (isFacingRight)
        {
            direction = 1f;
        }
        else
        {
            direction = -1f;
        }

        // Calculates the speed 
        rb.linearVelocity = new Vector2(direction * currentSlideSpeed, rb.linearVelocity.y);

        // This line makes sure we slow down after some time 
        currentSlideSpeed -= slideDrag * Time.deltaTime;


        // To finish off, this checks if the player is moving slower than X so we just stop sliding and stand up to normal position. 
        if (currentSlideSpeed <= 2f)
        {
            StopSlide();
        }
    }

    // Makes sure we cant slide forever, stops the player from permanently sliding. 
    private void StopSlide()
    {
        isSliding = false;
        // Brings back the player collider scale back to normal - standing position, 
        // because when we slide the player supposed to be smaller so the collider gets smaller accordingly 
        playerCollider.size = new Vector2(playerCollider.size.x, originalColliderHeight);
        playerCollider.offset = originalColliderOffset;
    }

    // This method gives the player a feeling that he has weight and is not a feather 
    public void Gravity()
    {
        if (isWallGrabbing)
        {
            return;
        }

        // This part checks if the player is falling down, < 0 (looks if the number is negative -> falling down) 
        if (rb.linearVelocity.y < 0)
        {
            // The falling speed gets multiplied which means the longer the fall the quicker the player goes down. 
            rb.gravityScale = baseGravity * fallSpeedMultiplier;

            // Makes sure the player cant fall like the speed of a meteor, we give a some kind of max value that cannot be crossed 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }

        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    // Checks if player is on the ground, which decides if the player can jump or not 
    private void GroundCheck()
    {
        bool wasGroundBefore = isGrounded;

        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

        // If the player is on the ground or touches it after jumping or etc, we give it back max jumps which is 2 
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        // Landing sound
        if (!wasGroundBefore && isGrounded)
        {
            sfxSource.PlayOneShot(PlayerLandingCave); // arba PlayerLandingGrass
        }
    }

    // Simple wall check - looks if the player collider is ON the wall 
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }


    // Method that allows player to grab a wall and hold onto it AS MUCH AS THE PLAYER WANTS, there is no timer or anything like that. 
    // If the player is holding any directional key to move next to a wall, the player will get "stuck" on to the wall 
    private void WallGrab()
    {

        // If the player is NOT on the ground and the collider is on the wall AND the player is moving left or right and is not sliding) 
        if (!isGrounded && WallCheck() && horizontalMovement != 0 && !isSliding)
        {
            // Gives a warning that the player is holding onto the wall to the code 
            isWallGrabbing = true;
            // Then we completely remove any movement that player has 
            rb.linearVelocity = Vector2.zero;
            // Then we shut off any gravity, so the player doesnt fly away or falls down 
            rb.gravityScale = 0f;
            // Calculates which way the player should jump  

            // For example if the player is holding onto the wall, and wants to jump into the same direction as he is holding, the code will force him jump away from it 
            wallJumpDirection = -transform.localScale.x;

            if (!wallHoldPlayed)
            {
                sfxSource.PlayOneShot(PlayerWallHold);
                wallHoldPlayed = true;
            }
            else
            {
                wallHoldPlayed = false;
            }
        }
        else
        {
            isWallGrabbing = false;
        }
    }

    // This method just checks which way the character is moving and calls FlipDirectly() method and flips the player object. 
    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            FlipDirectly();
        }
    }

    // Flips a Player model based on the way he is moving. 
    // Moves left - turn left, moves right - turn right. 
    private void FlipDirectly()
    {
        isFacingRight = !isFacingRight;
        // Creates a fake Player scale that we change. 
        Vector3 ls = transform.localScale;
        // Takes the Scale and multiplies by -1. 
        // Takes 1 (1 means he is moving right) and multiplies by -1 (-1 means moving left) 
        ls.x *= -1f;
        transform.localScale = ls;
    }

    // Draws visible boxes of GroundCheck and WallCheck around Player object 
    // so we can visually see, put it on and reshape as we like. 
    private void OnDrawGizmosSelected()
    {
        // GroundCheck visualization 
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        }
        // WallCheck visualization 
        if (wallCheckPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
        }
    }

    void HandleWalkingSound()
    {
        bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded && !isSliding;

        if (isWalking && !isWalkingSoundPlaying)
        {
            sfxSource.clip = PlayerWalkingCave; // arba PlayerWalkingGrass
            sfxSource.loop = true;
            sfxSource.Play();
            isWalkingSoundPlaying = true;
        }
        else if (!isWalking && isWalkingSoundPlaying)
        {
            sfxSource.Stop();
            isWalkingSoundPlaying = false;
        }
    }

}