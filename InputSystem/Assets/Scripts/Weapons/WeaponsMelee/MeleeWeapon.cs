using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Melee weapon — attacks enemies in a cone in front of the player
// Attack range and angle are configured in WeaponData
public class MeleeWeapon : WeaponBase
{
    private bool isAttacking = false;
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public override void Attack()
    {
        // Only trigger on button press, not hold
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (!CanAttack()) return;
        if (isAttacking) return;

        StartCoroutine(MeleeAttackCoroutine());
        OnAttackPerformed();
    }

    public override void StopAttack()
    {
        isAttacking = false;
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        isAttacking = true;
        animator?.SetTrigger("Attack");

        // Calculate attack center point in front of player
        Vector2 attackCenter = (Vector2)transform.position +
            (Vector2)transform.right * data.attackRange * 0.5f;

        // Find all enemies within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackCenter,
            data.attackRange,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider2D hit in hits)
        {
            // Only damage enemies within the attack angle cone
            Vector2 dirToEnemy = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.right, dirToEnemy);

            if (angle < data.attackAngle * 0.5f)
            {
                DamageSystem.ApplyDamage(hit.gameObject, CalculateDamage(), data.damageType);
                vfx?.PlayHitEffect(hit.transform.position, data.damageType);
            }
        }

        vfx?.PlayMeleeSlash(transform.position, data.attackRange);

        // Wait for fire rate before allowing next attack
        yield return new WaitForSeconds(data.fireRate);
        isAttacking = false;
    }

    // Visualizes the attack range in the Scene view (editor only)
    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + (Vector2)transform.right * data.attackRange * 0.5f;
        Gizmos.DrawWireSphere(center, data.attackRange);
    }
}