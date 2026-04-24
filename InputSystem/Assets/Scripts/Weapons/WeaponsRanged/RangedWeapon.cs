using UnityEngine;
using UnityEngine.InputSystem;

// Ranged weapon — fires bullets toward the mouse cursor
// Requires: BulletPrefab assigned, FirePoint child object set
public class RangedWeapon : WeaponBase
{
    public GameObject bulletPrefab;

    public override void Attack()
    {
        if (!CanAttack()) return;
        Shoot();
        OnAttackPerformed();
    }

    public override void StopAttack() { }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Get mouse position in world space using new Input System
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;

        // Spawn bullet and initialize it with direction and damage
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null) b.Initialize(direction, CalculateDamage(), data.damageType, vfx);

        vfx?.PlayMuzzleFlash();
    }
}