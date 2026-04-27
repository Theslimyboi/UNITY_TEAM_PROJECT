using UnityEngine;

public class ShooterEnemyAI : MonoBehaviour
{
    public Transform player;         // FIX: auto-found in Start() if not assigned
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float detectionRange = 8f;
    public float shootCooldown = 1.5f;
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;

    private Vector2 startPos;
    private bool movingRight = true;
    private float cooldownTimer = 0f;

    void Start()
    {
        startPos = transform.position;

        // FIX: auto-find player so it works even if not assigned in Inspector
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else Debug.LogWarning("ShooterEnemyAI: No Player found in scene!", this);
        }
    }

    void Patrol()
    {
        float leftLimit = startPos.x - patrolDistance;
        float rightLimit = startPos.x + patrolDistance;

        if (movingRight)
        {
            transform.position += Vector3.right * patrolSpeed * Time.deltaTime;
            if (transform.position.x >= rightLimit) movingRight = false;
        }
        else
        {
            transform.position += Vector3.left * patrolSpeed * Time.deltaTime;
            if (transform.position.x <= leftLimit) movingRight = true;
        }
    }

    void Update()
    {
        // FIX: null-check so no NullReferenceException if player is missing or dead
        if (player == null || !player.gameObject.activeInHierarchy) { Patrol(); return; }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            Patrol();
            return;
        }

        Vector2 dir = (player.position - shootPoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

        if (cooldownTimer <= 0f)
        {
            Shoot();
            cooldownTimer = shootCooldown;
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("ShooterEnemyAI: bulletPrefab not assigned!", this);
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Vector2 direction = (player.position - shootPoint.position).normalized;
        bullet.GetComponent<Bullet2>().SetDirection(direction);
    }
}