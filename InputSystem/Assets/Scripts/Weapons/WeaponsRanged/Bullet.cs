using UnityEngine;

// Attach to Bullet prefab.
// Requires: Rigidbody2D (Gravity Scale=0, Continuous), CircleCollider2D (Is Trigger=true)
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    private float damage;
    private DamageType damageType;
    private VFXController vfx;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3f);
    }

    public void Initialize(Vector2 dir, float dmg, DamageType type, VFXController vfxController)
    {
        damage = dmg;
        damageType = type;
        vfx = vfxController;
        rb.linearVelocity = dir * speed;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            DamageSystem.ApplyDamage(other.gameObject, damage, damageType);
            vfx?.PlayHitEffect(transform.position, damageType);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall")
             || other.gameObject.layer == LayerMask.NameToLayer("Wall")
             || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
        // Ignores everything else (other bullets, player, decorations)
    }
}