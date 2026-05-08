using UnityEngine;

public static class DamageSystem
{
    public static void ApplyDamage(GameObject target, float damage, DamageType damageType)
    {
        Health health = target.GetComponent<Health>();
        if (health == null) return;

        float finalDamage = CalculateDamageWithType(damage, damageType, target);
        health.TakeDamage(finalDamage);
    }

    private static float CalculateDamageWithType(float baseDamage, DamageType type, GameObject target)
    {
        float multiplier = 1f;
        EnemyStats stats = target.GetComponent<EnemyStats>();

        if (stats != null)
        {
            switch (type)
            {
                case DamageType.Fire:
                    multiplier = 1f / stats.fireResistance;
                    ApplyStatusEffect(target, StatusEffect.Burning, 3f);
                    break;
                case DamageType.Ice:
                    multiplier = 1f / stats.iceResistance;
                    ApplyStatusEffect(target, StatusEffect.Frozen, 2f);
                    break;
                case DamageType.Lightning:
                    multiplier = 1f / stats.lightningResistance;
                    break;
                case DamageType.Poison:
                    multiplier = 1f / stats.poisonResistance; // was ignoring resistance before
                    ApplyStatusEffect(target, StatusEffect.Poisoned, 5f);
                    break;
            }
        }

        return baseDamage * multiplier;
    }

    private static void ApplyStatusEffect(GameObject target, StatusEffect effect, float duration)
    {
        target.GetComponent<StatusEffectController>()?.ApplyEffect(effect, duration);
    }
}