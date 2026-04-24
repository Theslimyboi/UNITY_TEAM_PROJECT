using UnityEngine;

public class NPCMeleeEnemy : MonoBehaviour
{
    // Melee NPC status
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
    private float aggroTimer;
    private Vector2 lastKnownPlayerPos;

    private Transform currentTarget;
    private Transform player;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Give Player a tag so npc's can detect
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        currentTarget = pointB;
        currentState = EnemyState.Patrolling;
    }

    void FixedUpdate()
    {
        // Keep checking if npc can see the player
        bool canSeePlayerNow = IsPlayerInDetectionRange();

        if (canSeePlayerNow)
        {
            // If player is seen, reset timer and update last known position
            aggroTimer = aggroStayTime;
            lastKnownPlayerPos = player.position;
            currentState = EnemyState.Chasing;
        }
        else
        {
            // If player is lost, count down the timer
            if (aggroTimer > 0)
            {
                aggroTimer -= Time.fixedDeltaTime;
            }
            else if (currentState == EnemyState.Chasing)
            {
                // Only go back to patrolling when timer reaches zero
                currentState = EnemyState.Patrolling;
                SetReturnTarget();
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrolLogic(); // Removed parameter to keep it clean
                break;
            case EnemyState.Chasing:
                ChaseLogic(canSeePlayerNow);
                break;
        }
    }

    // Patrolling
    void PatrolLogic()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRayDistance, groundLayer);

        if (!isGroundAhead)
        {
            if (currentTarget == pointB)
            {
                currentTarget = pointA;
            }
            else
            {
                currentTarget = pointB;
            }
        }

        // Checking distance on X axis
        float distanceToTarget = Mathf.Abs(transform.position.x - currentTarget.position.x);

        if (distanceToTarget < 0.2f)
        {
            if (currentTarget == pointB)
            {
                currentTarget = pointA;
            }
            else
            {
                currentTarget = pointB;
            }
        }

        MoveTowards(currentTarget.position, patrolSpeed);
    }

    // Chasing the player logic
    void ChaseLogic(bool canSeePlayerNow)
    {
        Vector2 target = canSeePlayerNow ? (Vector2)player.position : lastKnownPlayerPos;

        float directionToTarget = target.x - transform.position.x;
        bool playerIsRight = directionToTarget > 0;

        
        Flip(playerIsRight);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRayDistance, groundLayer);
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, wallRayDistance, groundLayer);

        if (isGroundAhead && !isWallAhead)
        {
            MoveTowards(target, chaseSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (!canSeePlayerNow && Mathf.Abs(directionToTarget) > detectionRange / 2)
            {
                aggroTimer = 0;
            }
        }
    }

    // Helper to decide where to go back after chasing
    void SetReturnTarget()
    {
        if (Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position))
        {
            currentTarget = pointA;
        }
        else
        {
            currentTarget = pointB;
        }
    }

    // Npc moving
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

        // Flip sprite so it matches the moving direction
        Flip(moveDir > 0);
    }

    // Player detection
    bool IsPlayerInDetectionRange()
    {
        if (player == null)
        {
            return false;
        }

        // Calculate which direction is the player at
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Checking if the player is in the range of vision
        if (distanceToPlayer < detectionRange)
        {
            // Checking if the player is behind another npc
            Vector2 facingDirection = isFacingRight ? Vector2.right : Vector2.left;

            float angle = Vector2.Angle(facingDirection, directionToPlayer);

            // If chasing, widen vision angle to 180 (half circle) so jumps don't break aggro
            float currentMaxAngle = (currentState == EnemyState.Chasing) ? 180f : 90f;

            if (angle < currentMaxAngle)
            {
                // We direct the vision to the player and check if there is a wall between the player and npc
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, groundLayer | playerLayer);

                if (hit.collider != null)
                {
                    // If the vision "indicator" detects player first, then there is no wall between them
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Fliping sprite
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

    // Visuals for npc vision, patroling range
    private void OnDrawGizmos()
    {
        // Yellow laser is the path route, from point A to point B
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // When npc finds a player, red laser appears towards the player to see the vision, when aggro is on
        if (player != null && currentState == EnemyState.Chasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, lastKnownPlayerPos);
        }

        // Blue sphere around is the real vision range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Green laser checks for ground
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            Debug.Log("Player took melee damage!");
        }
    }

}