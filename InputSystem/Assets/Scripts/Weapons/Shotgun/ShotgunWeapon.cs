using UnityEngine;
using UnityEngine.InputSystem;

// Shotgun with recoil-based movement
// Attach to Shotgun GameObject under Player
public class ShotgunWeapon : WeaponBase
{
    [Header("Shotgun Settings")]
    public int pelletCount = 6;          // Number of bullets per shot
    public float spreadAngle = 30f;      // Total spread in degrees
    public GameObject bulletPrefab;

    [Header("Recoil Settings")]
    public float recoilForce = 15f;      // How strong the knockback is
    public bool recoilCancelVelocity = false; // Cancel current velocity before recoil

    private Rigidbody2D playerRb;

    void Awake()
    {
        // Find player Rigidbody2D by going up the hierarchy
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    public override void Attack()
    {
        if (!CanAttack()) return;
        Shoot();
        ApplyRecoil();
        OnAttackPerformed();
    }

    public override void StopAttack() { }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 baseDirection = (mousePos - (Vector2)firePoint.position).normalized;

        // Spawn multiple pellets with spread
        for (int i = 0; i < pelletCount; i++)
        {
            // Calculate spread offset for each pellet
            float spreadOffset = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Vector2 spreadDir = RotateVector(baseDirection, spreadOffset);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null) b.Initialize(spreadDir, CalculateDamage(), data.damageType, vfx);
        }

        vfx?.PlayMuzzleFlash();
    }

    private void ApplyRecoil()
    {
        if (playerRb == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);

        // Recoil goes OPPOSITE to shoot direction
        Vector2 shootDirection = (mousePos - (Vector2)transform.position).normalized;
        Vector2 recoilDirection = -shootDirection;

        // Cancel current velocity for consistent recoil feel
        if (recoilCancelVelocity)
            playerRb.linearVelocity = Vector2.zero;

        playerRb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
    }

    // Rotates a 2D vector by given degrees
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        );
    }
}