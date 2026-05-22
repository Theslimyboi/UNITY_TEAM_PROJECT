using UnityEngine;

// ScriptableObject that stores all weapon stats
// Create via: right-click in Assets > Create > Weapons > WeaponData
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("General Info")]
    public string weaponName = "Weapon";
    public Sprite weaponSprite;
    public WeaponType weaponType;

    [Header("Damage Settings")]
    public float baseDamage = 10f;
    public float critChance = 0.1f;         // 0.0 - 1.0 (10% default)
    public float critMultiplier = 2f;        // Damage multiplier on crit
    public DamageType damageType;

    [Header("Ammo / Fire Rate")]
    public int maxAmmo = 12;          // kulkos vienoje apkaboje
    public int totalAmmo = 36;        // bendras šovinių kiekis (pvz. 3 apkabos)
    public float fireRate = 0.2f;            // Seconds between attacks
    public float reloadTime = 1.5f;

    [Header("Melee Settings")]
    public float attackRange = 1.5f;
    public float attackAngle = 60f;          // Attack cone in degrees

    [Header("Magic Settings")]
    public float manaCost = 10f;
    public float cooldownTime = 2f;
    public GameObject spellPrefab;           // Projectile spawned when casting

    //[Header("Ammo Usage")]
    //public int ammoPerShot = 1; // jei 0, reiškia naudoti pelletCount

    [Header("Weapon Flags")]
    public bool isShotgun = false;
}