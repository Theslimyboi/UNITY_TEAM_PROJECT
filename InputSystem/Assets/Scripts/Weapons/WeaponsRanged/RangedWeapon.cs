using UnityEngine;
using UnityEngine.InputSystem;

// Works for Pistol, SMG, or any single-shot ranged weapon.
// Assign different WeaponData ScriptableObjects to get different fire rates / damage.
// Hierarchy: Player > [WeaponName] (this script + SpriteRenderer) > FirePoint
public class RangedWeapon : WeaponBase
{
    public GameObject bulletPrefab;

    private SpriteRenderer sr;
    private Transform playerTransform;
    //private AmmoSystem ammo;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        playerTransform = transform.parent;
        ammo = GetComponent<AmmoSystem>();
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

        bool playerFacingLeft = playerTransform != null && playerTransform.localScale.x < 0f;
        if (sr != null) sr.flipY = playerFacingLeft ? toMouse.x < 0f : toMouse.x > 0f;

        float angle = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg + 180f;
        if (playerFacingLeft) angle += 180f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public override void Attack()
    {
        if (!CanAttack()) return;
        Shoot();
        OnAttackPerformed();
    }

    public override void StopAttack() { }

    /*
    private void Shoot()
    {
        /*if (ammo != null && !ammo.HasAmmo())
            return; // nėra kulkų — nešaudom

        

        if (bulletPrefab == null || firePoint == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        bool facingLeft = playerTransform != null && playerTransform.localScale.x < 0f;
        Vector2 firePointPos = firePoint.position;

        if (facingLeft)
            firePointPos.y = transform.position.y - (firePoint.position.y - transform.position.y);

        Vector2 direction = (mouseWorld - firePointPos).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePointPos, Quaternion.identity);
        bullet.GetComponent<Bullet>()?.Initialize(direction, CalculateDamage(), data.damageType, vfx);
        vfx?.PlayMuzzleFlash();

    if (ammo != null && !ammo.HasAmmo()) return;
    if (bulletPrefab == null || firePoint == null) return;

    Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    bool facingLeft = playerTransform != null && playerTransform.localScale.x < 0f;

    
    Vector3 localPos = transform.InverseTransformPoint(firePoint.position);
    if (facingLeft) localPos.y = -localPos.y;
    Vector2 firePointPos = transform.TransformPoint(localPos);

    Vector2 direction = (mouseWorld - firePointPos).normalized;

    GameObject bullet = Instantiate(bulletPrefab, firePointPos, Quaternion.identity);
    bullet.GetComponent<Bullet>()?.Initialize(direction, CalculateDamage(), data.damageType, vfx);
    vfx?.PlayMuzzleFlash(firePointPos, transform.rotation);
}
    */

    private void Shoot()
    {
        if (ammo != null && !ammo.HasAmmo()) return;
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Get the barrel length (X distance of firePoint in local space only)
        float barrelLength = Mathf.Abs(firePoint.localPosition.x);

        // Calculate aim direction purely from mouse
        Vector2 aimDir = (mouseWorld - (Vector2)transform.position).normalized;

        // Muzzle is always exactly barrelLength units along aim direction
        Vector2 firePointPos = (Vector2)transform.position + aimDir * barrelLength + Vector2.up * firePoint.localPosition.y;

        Vector2 direction = aimDir;

        GameObject bullet = Instantiate(bulletPrefab, firePointPos, Quaternion.identity);
        bullet.GetComponent<Bullet>()?.Initialize(direction, CalculateDamage(), data.damageType, vfx);
        vfx?.PlayMuzzleFlash(firePointPos, transform.rotation);
    }
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ammo?.StartReload();
    }


}