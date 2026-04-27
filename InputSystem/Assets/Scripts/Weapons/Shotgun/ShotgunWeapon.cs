using UnityEngine;
using UnityEngine.InputSystem;

// Shotgun with spread, recoil, and full VFX support.
// Attach to Shotgun GameObject under Player.
//
// Inspector checklist:
//   data            -> ShotgunData ScriptableObject
//   firePoint       -> FirePoint child transform
//   vfx             -> VFXController on this same GameObject
//   ammo            -> AmmoSystem on this same GameObject (or null = infinite)
//   bulletPrefab    -> ShotgunPellet prefab  (NOT the pistol Bullet prefab)
//   muzzleFlashPrefab (on VFXController) -> assign a flash sprite/particle prefab

public class ShotgunWeapon : WeaponBase
{
    [Header("Shotgun Settings")]
    public int pelletCount = 6;      // pellets fired per shot
    public float spreadAngle = 30f;    // total spread cone in degrees

    // bulletPrefab must use ShotgunPellet, not Bullet
    public GameObject bulletPrefab;

    [Header("Recoil Settings")]
    public float recoilForce = 15f;
    public bool recoilCancelVelocity = false;

    private Rigidbody2D playerRb;

    // ── Unity lifecycle ────────────────────────────────────────────────────────

    void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    // ── WeaponBase implementation ──────────────────────────────────────────────

    public override void Attack()
    {
        if (!CanAttack()) return;
        Shoot();
        ApplyRecoil();
        OnAttackPerformed();   // handles fire-rate timer + ammo consumption
    }

    public override void StopAttack() { }

    // ── Private helpers ────────────────────────────────────────────────────────

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("ShotgunWeapon: bulletPrefab not assigned!", this);
            return;
        }
        if (firePoint == null)
        {
            Debug.LogWarning("ShotgunWeapon: firePoint not assigned!", this);
            return;
        }

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 baseDir = (mouseWorld - (Vector2)firePoint.position).normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            float offset = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Vector2 spreadDir = RotateVector(baseDir, offset);

            GameObject go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // Support both pellet types: the new ShotgunPellet or the generic Bullet
            ShotgunPellet sp = go.GetComponent<ShotgunPellet>();
            if (sp != null)
            {
                sp.Initialize(spreadDir, CalculateDamage(), data.damageType, vfx);
            }
            else
            {
                Bullet b = go.GetComponent<Bullet>();
                if (b != null) b.Initialize(spreadDir, CalculateDamage(), data.damageType, vfx);
            }
        }

        // Muzzle flash (handled by VFXController)
        vfx?.PlayMuzzleFlash();
    }

    private void ApplyRecoil()
    {
        if (playerRb == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 shootDir = (mouseWorld - (Vector2)transform.position).normalized;
        Vector2 recoilDir = -shootDir;

        if (recoilCancelVelocity)
            playerRb.linearVelocity = Vector2.zero;

        playerRb.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);
    }

    /// <summary>Rotates a 2D vector by the given angle in degrees.</summary>
    private static Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}