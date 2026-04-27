using UnityEngine;

// Attach to the ShotgunPellet prefab.
// Prefab setup:
//   - Sprite: small oval/circle (white or pale yellow)
//   - Rigidbody2D: Gravity Scale = 0, Collision Detection = Continuous
//   - CircleCollider2D: Is Trigger = true, radius ~0.06
//   - TrailRenderer (optional but recommended — see below)
//   - This script
//
// TrailRenderer settings (on the prefab):
//   Time: 0.06     Width: 0.08 -> 0
//   Material: Sprites-Default, Color: white -> transparent
//   Min Vertex Distance: 0.05

public class ShotgunPellet : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────────

    [Header("Movement")]
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifetime = 0.35f;   // pellets die fast

    [Header("VFX")]
    [SerializeField] private GameObject impactEffectPrefab;   // particle burst on hit
    [SerializeField] private TrailRenderer trail;             // assign in prefab

    // ── Runtime state ──────────────────────────────────────────────────────────

    private float damage;
    private DamageType damageType;
    private VFXController vfx;
    private Rigidbody2D rb;

    // ── Unity lifecycle ────────────────────────────────────────────────────────

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by ShotgunWeapon right after Instantiate.
    /// </summary>
    public void Initialize(Vector2 direction, float dmg, DamageType type, VFXController vfxController)
    {
        damage = dmg;
        damageType = type;
        vfx = vfxController;

        // Velocity
        rb.linearVelocity = direction * speed;

        // Rotate sprite to face travel direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // ── Collision ──────────────────────────────────────────────────────────────

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            DamageSystem.ApplyDamage(other.gameObject, damage, damageType);
            SpawnImpact();
            vfx?.PlayHitEffect(transform.position, damageType);
            DestroyPellet();
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            SpawnImpact();
            DestroyPellet();
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private void SpawnImpact()
    {
        if (impactEffectPrefab == null) return;

        // Detach trail so it finishes fading naturally, then destroy it
        if (trail != null)
        {
            trail.transform.SetParent(null);
            Destroy(trail.gameObject, trail.time + 0.1f);
        }

        GameObject fx = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 1f);
    }

    private void DestroyPellet()
    {
        // Detach trail before destroying so it can finish fading
        if (trail != null)
        {
            trail.transform.SetParent(null);
            Destroy(trail.gameObject, trail.time + 0.1f);
        }
        Destroy(gameObject);
    }
}