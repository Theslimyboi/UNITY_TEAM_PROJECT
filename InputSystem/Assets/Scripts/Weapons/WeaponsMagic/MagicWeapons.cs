using UnityEngine;
using UnityEngine.InputSystem;

// Magic weapon — casts spells that deal area damage
// Requires: SpellPrefab assigned in WeaponData, FirePoint set
public class MagicWeapon : WeaponBase
{
    public float currentMana = 100f;
    public float maxMana = 100f;
    public float manaRegenRate = 5f; // Mana regenerated per second

    void Update()
    {
        // Regenerate mana over time
        if (currentMana < maxMana)
            currentMana = Mathf.Min(maxMana, currentMana + manaRegenRate * Time.deltaTime);
    }

    public override void Attack()
    {
        // Only cast on button press, not hold
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (!CanAttack()) return;
        if (currentMana < data.manaCost) { Debug.Log("Not enough mana!"); return; }

        CastSpell();
        OnAttackPerformed();
    }

    public override void StopAttack() { }

    private void CastSpell()
    {
        if (data.spellPrefab == null || firePoint == null) return;

        // Get mouse position in world space using new Input System
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;

        // Spawn spell and initialize it
        GameObject spell = Instantiate(data.spellPrefab, firePoint.position, Quaternion.identity);
        Spell s = spell.GetComponent<Spell>();
        if (s != null) s.Initialize(direction, CalculateDamage(), data.damageType, vfx);

        currentMana -= data.manaCost;
        vfx?.PlayMuzzleFlash();
        Debug.Log($"Spell cast! Mana remaining: {currentMana:F0}/{maxMana}");
    }

    // Returns mana as a 0-1 value for UI use
    public float GetManaPercent() => currentMana / maxMana;
}

// Attach to the Spell prefab
// Requires: Rigidbody2D, Collider2D (Is Trigger = true)
public class Spell : MonoBehaviour
{
    private float damage;
    private DamageType damageType;
    private VFXController vfx;

    [SerializeField] private float speed = 8f;

    void Awake() { Destroy(gameObject, 5f); }

    // Called by MagicWeapon after spawning the spell
    public void Initialize(Vector2 dir, float dmg, DamageType type, VFXController vfxController)
    {
        damage = dmg;
        damageType = type;
        vfx = vfxController;
        GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
    }

    // On hit — damages all enemies in a small radius (area effect)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
                DamageSystem.ApplyDamage(hit.gameObject, damage, damageType);

            vfx?.PlayHitEffect(transform.position, damageType);
            Destroy(gameObject);
        }
    }
}