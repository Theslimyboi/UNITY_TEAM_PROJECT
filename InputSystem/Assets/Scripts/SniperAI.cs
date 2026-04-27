using UnityEngine;

public class SniperAI : MonoBehaviour
{
    public Transform player;
    public Transform gunPoint;
    public LineRenderer laser;

    public float detectionRange = 3f;
    public float aimTime = 1.5f;
    public float shootCooldown = 2f;

    private bool isAiming = false;
    private float aimTimer = 0f;
    private float cooldownTimer = 0f;
    private float flashTimer = 0f;

    void Start()
    {
        laser.enabled = false;
        SetLaserWidth(0.05f);
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;
        float distance = Vector2.Distance(gunPoint.position, player.position);

        
        if (distance < detectionRange && cooldownTimer <= 0f && !isAiming)
        {
            isAiming = true;
            aimTimer = aimTime;
            laser.enabled = true;
            SetLaserWidth(0.05f);
        }

        if (distance > detectionRange)
        {
            isAiming = false;
            laser.enabled = false;
        }

        if (isAiming)
        {
            UpdateLaser();
            aimTimer -= Time.deltaTime;

            if (aimTimer <= 0f)
                Shoot();
        }
        
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
       
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0f)
            {
                SetLaserWidth(0.05f);
                laser.enabled = false;
            }
        }
    }

    void Shoot()
    {
        
        laser.enabled = true;
        SetLaserWidth(0.5f);
        UpdateLaser();

        flashTimer = 0.1f;

        int mask = ~LayerMask.GetMask("Enemy");

        RaycastHit2D hit = Physics2D.Raycast(
            gunPoint.position,
            (player.position - gunPoint.position).normalized, detectionRange,
             mask
        );

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Sniper hit the player!");
            hit.collider.GetComponent<PlayerHealth>().TakeDamage(2);
        }
        else
        {
            Debug.Log("Shot blocked by obstacle.");
        }

        isAiming = false;
        cooldownTimer = shootCooldown;
    }

    void UpdateLaser()
    {
        Vector2 direction = (player.position - gunPoint.position).normalized;

        int mask = ~LayerMask.GetMask("Enemy");

        RaycastHit2D hit = Physics2D.Raycast(
            gunPoint.position,
            direction,
            detectionRange,
            mask
        );

        laser.SetPosition(0, gunPoint.position);

        if (hit.collider != null)
        {
            laser.SetPosition(1, hit.point);
        }
        else
        {
            laser.SetPosition(1, player.position);
        }
    }

    void SetLaserWidth(float w)
    {
        laser.startWidth = w;
        laser.endWidth = w;
    }
}
