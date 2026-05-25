using UnityEngine;

// NPCMeleeEnemy - patrols between two points, chases and damages the player on contact.
//
// IMPORTANT SETUP NOTE:
// This script deals damage via OnTriggerEnter2D. For this to fire, the enemy's
// collider must have "Is Trigger" = TRUE. But a trigger collider has no physics -
// the enemy will fall through the floor.
//
// RECOMMENDED FIX: Use OnCollisionEnter2D instead (see bottom of this file),
// keep "Is Trigger" = FALSE on the enemy's main collider. Add a SEPARATE child
// GameObject with a slightly larger trigger collider for the damage zone.
// This script uses OnCollisionEnter2D so the enemy can stand on the ground.

public class NPCMeleeEnemy : MonoBehaviour
{
    private enum EnemyState { Patrolling, Chasing }
    private EnemyState currentState;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public Transform pointA;
    public Transform pointB;

    [Header("Detection")]
    public float detectionRange = 5f;
    public LayerMask playerLayer;
    public Transform wallCheck;
    public float wallRayDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Edge Detection")]
    public Transform groundCheck;
    public float groundRayDistance = 1.5f;

    [Header("Aggro Settings")]
    public float aggroStayTime = 2f;

    [Header("Damage")]
    public int contactDamage = 1;          // FIX: exposed so you can tune it in Inspector
    public float damageCooldown = 1f;      // FIX: prevents hitting player every frame on contact
    private float damageTimer = 0f;

    [Header("Audio Settings")]
    public AudioSource audioSource;        // pagrindinis garso šaltinis
    public AudioClip walkClip;             // žingsnių garsas
    public float walkThreshold = 0.1f;     // greičio slenkstis, nuo kurio groja žingsniai


    private float aggroTimer;
    private Vector2 lastKnownPlayerPos;
    private Transform currentTarget;
    private Transform player;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        currentTarget = pointB;
        currentState = EnemyState.Patrolling;
    }

    void FixedUpdate()
    {
        bool canSeePlayerNow = IsPlayerInDetectionRange();

        if (canSeePlayerNow)
        {
            aggroTimer = aggroStayTime;
            lastKnownPlayerPos = player.position;
            currentState = EnemyState.Chasing;
        }
        else
        {
            if (aggroTimer > 0)
                aggroTimer -= Time.fixedDeltaTime;
            else if (currentState == EnemyState.Chasing)
            {
                currentState = EnemyState.Patrolling;
                SetReturnTarget();
            }
        }

        // FIX: tick damage cooldown in FixedUpdate
        if (damageTimer > 0)
            damageTimer -= Time.fixedDeltaTime;

        switch (currentState)
        {
            case EnemyState.Patrolling: PatrolLogic(); break;
            case EnemyState.Chasing: ChaseLogic(canSeePlayerNow); break;
        }

        HandleFootsteps();
    }

    void PatrolLogic()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRayDistance, groundLayer);
        if (!isGroundAhead)
            currentTarget = (currentTarget == pointB) ? pointA : pointB;

        float distanceToTarget = Mathf.Abs(transform.position.x - currentTarget.position.x);
        if (distanceToTarget < 0.2f)
            currentTarget = (currentTarget == pointB) ? pointA : pointB;

        MoveTowards(currentTarget.position, patrolSpeed);
    }

    void ChaseLogic(bool canSeePlayerNow)
    {
        Vector2 target = canSeePlayerNow ? (Vector2)player.position : lastKnownPlayerPos;
        float directionToTarget = target.x - transform.position.x;
        bool playerIsRight = directionToTarget > 0;
        Flip(playerIsRight);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRayDistance, groundLayer);
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, wallRayDistance, groundLayer);

        if (isGroundAhead && !isWallAhead)
            MoveTowards(target, chaseSpeed);
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (!canSeePlayerNow && Mathf.Abs(directionToTarget) > detectionRange / 2)
                aggroTimer = 0;
        }
    }

    void SetReturnTarget()
    {
        currentTarget = (Vector2.Distance(transform.position, pointA.position) <
                         Vector2.Distance(transform.position, pointB.position))
                        ? pointA : pointB;
    }

    void MoveTowards(Vector2 targetPos, float speed)
    {
        float directionX = targetPos.x - transform.position.x;
        if (Mathf.Abs(directionX) < 0.1f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        float moveDir = (directionX > 0) ? 1 : -1;
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);
        Flip(moveDir > 0);
    }

    bool IsPlayerInDetectionRange()
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            Vector2 facingDirection = isFacingRight ? Vector2.right : Vector2.left;
            float angle = Vector2.Angle(facingDirection, directionToPlayer);
            float currentMaxAngle = (currentState == EnemyState.Chasing) ? 180f : 90f;

            if (angle < currentMaxAngle)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, groundLayer | playerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                    return true;
            }
        }
        return false;
    }

    void Flip(bool faceRight)
    {
        if (isFacingRight != faceRight)
        {
            isFacingRight = faceRight;
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }
    }

    // FIX: use OnCollisionStay2D (not OnTriggerEnter2D) so the enemy's collider
    // can stay non-trigger and still stand on the ground.
    // Also added damageCooldown so the player isn't hit every frame.
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && damageTimer <= 0f)
        {
            collision.collider.GetComponent<PlayerHealth>()?.TakeDamage(contactDamage);
            damageTimer = damageCooldown;
            Debug.Log("Player took melee damage!");
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
        if (player != null && currentState == EnemyState.Chasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, lastKnownPlayerPos);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundRayDistance);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallRayDistance);
        }
    }

    private void HandleFootsteps()
    {
        if (audioSource == null || walkClip == null) return;

        // --- DISTANCE CHECK ---
        float dist = Vector2.Distance(transform.position, player.position);

        // girdima tik jei žaidėjas yra arti (pvz. 12f)
        if (dist > 12f)
        {
            // jei žaidėjas toli — nutildom žingsnius
            if (audioSource.isPlaying && audioSource.clip == walkClip)
                audioSource.Stop();
            return;
        }

        // --- FOOTSTEP LOGIC ---
        if (Mathf.Abs(rb.linearVelocity.x) > walkThreshold)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = walkClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == walkClip)
                audioSource.Stop();
        }
    }


}