using UnityEngine;
using UnityEngine.InputSystem;

// Shotgun with spread, recoil, full VFX support, and mouse aiming.
// Attach to Shotgun GameObject under Player.
// Inspector checklist:
//   data            -> ShotgunData ScriptableObject
//   firePoint       -> FirePoint child transform (place at barrel tip)
//   vfx             -> VFXController on this same GameObject
//   ammo            -> AmmoSystem on this same GameObject (or null = infinite)
//   bulletPrefab    -> ShotgunPellet prefab

public class ShotgunWeapon : WeaponBase
{
    [Header("Shotgun Settings")]
    public int pelletCount = 6;
    public float spreadAngle = 30f;
    public GameObject bulletPrefab;

    [Header("Recoil Settings")]
    public float recoilForce = 15f;
    public bool recoilCancelVelocity = false;

    private Rigidbody2D playerRb;
    private SpriteRenderer sr;

    void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        if (Camera.main == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 toMouse = mouseWorld - (Vector2)transform.position;

        bool playerFacingLeft = transform.parent != null && transform.parent.localScale.x < 0f;

        if (sr != null) sr.flipY = playerFacingLeft ? toMouse.x < 0f : toMouse.x > 0f;

        float angle = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg + 180f;

        // When player faces left, add another 180° so the gun still tracks the mouse correctly
        if (playerFacingLeft) angle += 180f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
        if (bulletPrefab == null) { Debug.LogWarning("ShotgunWeapon: bulletPrefab not assigned!", this); return; }
        if (firePoint == null) { Debug.LogWarning("ShotgunWeapon: firePoint not assigned!", this); return; }

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 baseDir = (mouseWorld - (Vector2)firePoint.position).normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            float offset = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Vector2 spreadDir = RotateVector(baseDir, offset);

            GameObject go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            ShotgunPellet sp = go.GetComponent<ShotgunPellet>();
            if (sp != null)
                sp.Initialize(spreadDir, CalculateDamage(), data.damageType, vfx);
            else
                go.GetComponent<Bullet>()?.Initialize(spreadDir, CalculateDamage(), data.damageType, vfx);
        }

        vfx?.PlayMuzzleFlash();
    }

    private void ApplyRecoil()
    {
        if (playerRb == null) return;
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Recoil is opposite to shoot direction, horizontal only so it doesn't push player up/down
        Vector2 shootDir = (mouseWorld - (Vector2)firePoint.position).normalized;
        Vector2 recoilDir = -shootDir;
        if (recoilCancelVelocity) playerRb.linearVelocity = Vector2.zero;
        playerRb.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);
    }

    private static Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}