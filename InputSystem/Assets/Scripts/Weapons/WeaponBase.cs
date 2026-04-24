using UnityEngine;

// Weapon types available in the game
public enum WeaponType { Ranged, Melee, Magic }

// Damage types that affect enemies differently
public enum DamageType { Physical, Fire, Ice, Lightning, Poison }

// Abstract base class for all weapons
// Every weapon type (Ranged, Melee, Magic) must inherit from this
public abstract class WeaponBase : MonoBehaviour
{
    [Header("References (assign in Unity Inspector)")]
    public WeaponData data;         // Weapon stats and settings
    public Transform firePoint;     // Point from which attacks originate
    public VFXController vfx;       // Visual effects controller
    public AmmoSystem ammo;         // Ammo and reload system

    protected float nextFireTime = 0f;
    protected bool isReloading = false;

    // Must be implemented by every weapon subclass
    public abstract void Attack();
    public abstract void StopAttack();

    // Checks all conditions before allowing an attack
    protected bool CanAttack()
    {
        if (isReloading) return false;
        if (Time.time < nextFireTime) return false;
        if (ammo != null && !ammo.HasAmmo()) return false;
        return true;
    }

    // Called after every successful attack to update fire rate and ammo
    protected void OnAttackPerformed()
    {
        nextFireTime = Time.time + data.fireRate;
        ammo?.UseAmmo();
    }

    // Calculates damage including critical hit chance
    protected float CalculateDamage()
    {
        bool isCrit = Random.value < data.critChance;
        float damage = data.baseDamage;
        if (isCrit)
        {
            damage *= data.critMultiplier;
            Debug.Log($"Critical hit! Damage: {damage}");
        }
        return damage;
    }
}