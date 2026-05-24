using UnityEngine;

public class SniperAI : MonoBehaviour
{
    public Transform player;
    public Transform gunPoint;
    public LineRenderer laser;

    [Header("Range")]
    public float detectionRange = 8f;
    public float shootRange = 20f;

    [Header("Timing")]
    public float aimTime = 1.5f;
    public float shootCooldown = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sniperLockOnClip;     
    public AudioClip sniperShootClip;      

    private bool isAiming = false;
    private float aimTimer = 0f;
    private float cooldownTimer = 0f;
    private float flashTimer = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        laser.enabled = false;
        SetLaserWidth(0.05f);
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        float distance = Vector2.Distance(gunPoint.position, player.position);

        // --- FIKSAVIMO MOMENTAS ---
        if (distance < detectionRange && cooldownTimer <= 0f && !isAiming)
        {
            isAiming = true;
            aimTimer = aimTime;
            laser.enabled = true;
            SetLaserWidth(0.05f);

            // GROJAM FIKSAVIMO GARSĄ
            if (audioSource != null && sniperLockOnClip != null)
                audioSource.PlayOneShot(sniperLockOnClip);
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

        // --- ŠŪVIO GARSAS ---
        if (audioSource != null && sniperShootClip != null)
            audioSource.PlayOneShot(sniperShootClip);

        int mask = ~LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(
            gunPoint.position,
            (player.position - gunPoint.position).normalized,
            shootRange,
            mask
        );

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            hit.collider.GetComponent<PlayerHealth>().TakeDamage(2);
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
            shootRange,
            mask
        );

        laser.SetPosition(0, gunPoint.position);
        laser.SetPosition(1, hit.collider != null ? hit.point : (Vector2)player.position);
    }

    void SetLaserWidth(float w)
    {
        laser.startWidth = w;
        laser.endWidth = w;
    }
}
