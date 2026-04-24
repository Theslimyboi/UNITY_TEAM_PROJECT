using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

// Attach to each weapon GameObject
// Handles visual effects: muzzle flash, bullet trail, hit effects
public class VFXController : MonoBehaviour
{
    [Header("Muzzle Flash")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;

    [Header("Bullet Trail")]
    public GameObject bulletTrailPrefab;
    public float trailDuration = 0.05f;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;
    public GameObject[] damageTypeEffects; // Optional: different effect per damage type

    // Spawns a muzzle flash at the fire point
    public void PlayMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) return;
        GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(flash, 0.1f);
    }

    // Draws a trail from the fire point to the target position
    public void PlayBulletTrail(Vector2 from, Vector2 to)
    {
        if (bulletTrailPrefab == null) return;
        GameObject trail = Instantiate(bulletTrailPrefab, from, Quaternion.identity);
        trail.transform.right = (to - from).normalized;
        trail.transform.localScale = new Vector3(Vector2.Distance(from, to), 0.1f, 1f);
        Destroy(trail, trailDuration);
    }

    // Spawns a hit effect based on damage type
    public void PlayHitEffect(Vector2 position, DamageType damageType)
    {
        int typeIndex = (int)damageType;
        GameObject effectPrefab = null;

        if (damageTypeEffects != null && typeIndex < damageTypeEffects.Length)
            effectPrefab = damageTypeEffects[typeIndex];
        else
            effectPrefab = hitEffectPrefab;

        if (effectPrefab == null) return;
        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(effect, 1f);
    }

    // Called for melee attacks — add slash animation here
    [Header("Melee Slash")]
    public GameObject meleeSlashPrefab;

    public void PlayMeleeSlash(Vector2 position, float radius)
    {
        if (meleeSlashPrefab == null) return;

        // Spawn slash facing mouse direction
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = (mousePos - position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject slash = Instantiate(meleeSlashPrefab, position, rotation);
        slash.transform.localScale = new Vector3(radius, radius, 1f);
    }
}

