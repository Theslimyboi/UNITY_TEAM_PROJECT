using UnityEngine;

public class NPCMeleeWeapon : WeaponBase
{
    public override void Attack()
    {
        if (!CanAttack()) return;
        // Enemy can attack automatically no need for the input
        PerformAttack();
        OnAttackPerformed();
    }

    public override void StopAttack() { }

    private void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            data.attackRange,
            LayerMask.GetMask("Player") // Attacks the player
        );

        foreach (Collider2D hit in hits)
        {
            DamageSystem.ApplyDamage(hit.gameObject, CalculateDamage(), data.damageType);
        }
    }
}