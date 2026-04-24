using UnityEngine;

// Attach to the Bullet prefab
// Requires: Rigidbody2D, CircleCollider2D (Is Trigger = true)
public class Bullet : MonoBehaviour
{
    private float damage;
    private DamageType damageType;
    private VFXController vfx;

    [SerializeField] private float speed = 15f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3f); // Auto-destroy after 3 seconds
    }

    // Called by RangedWeapon after spawning the bullet
    public void Initialize(Vector2 dir, float dmg, DamageType type, VFXController vfxController)
    {
        damage = dmg;
        damageType = type;
        vfx = vfxController;

        rb.linearVelocity = dir * speed;

        // Rotate bullet sprite to face movement direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            DamageSystem.ApplyDamage(other.gameObject, damage, damageType);
            vfx?.PlayHitEffect(transform.position, damageType);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}