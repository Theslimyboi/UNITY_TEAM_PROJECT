using UnityEngine;

public enum WeaponType { Ranged, Melee, Magic }
public enum DamageType { Physical, Fire, Ice, Lightning, Poison }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("References (assign in Unity Inspector)")]
    public WeaponData data;
    public Transform firePoint;
    public VFXController vfx;
    public AmmoSystem ammo;

    protected float nextFireTime = 0f;

    public abstract void Attack();
    public abstract void StopAttack();

    protected bool CanAttack()
    {
        if (data == null) return false;
        if (Time.time < nextFireTime) return false;
        if (ammo != null && !ammo.HasAmmo()) return false;
        return true;
    }

    protected void OnAttackPerformed()
    {
        nextFireTime = Time.time + data.fireRate;
        ammo?.UseAmmo();
    }

    protected float CalculateDamage()
    {
        bool isCrit = Random.value < data.critChance;
        float damage = data.baseDamage;
        if (isCrit) damage *= data.critMultiplier;
        return damage;
    }
}