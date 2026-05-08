using UnityEngine;
using System.Collections;

public enum StatusEffect { Burning, Frozen, Poisoned, Stunned }

// Attach to enemies to handle status effects properly
public class StatusEffectController : MonoBehaviour
{
    private Health health;
    private Rigidbody2D rb;

    [Header("Effect Settings")]
    public float burnDamagePerTick = 3f;
    public float burnTickRate = 0.5f;
    public float freezeSlowAmount = 0.4f; // multiplier on movement speed
    public float poisonDamagePerTick = 1.5f;
    public float poisonTickRate = 1f;

    void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyEffect(StatusEffect effect, float duration)
    {
        switch (effect)
        {
            case StatusEffect.Burning: StartCoroutine(BurnCoroutine(duration)); break;
            case StatusEffect.Frozen: StartCoroutine(FreezeCoroutine(duration)); break;
            case StatusEffect.Poisoned: StartCoroutine(PoisonCoroutine(duration)); break;
        }
    }

    private IEnumerator BurnCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(burnTickRate);
            health?.TakeDamage(burnDamagePerTick);
            elapsed += burnTickRate;
        }
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(duration);
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private IEnumerator PoisonCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(poisonTickRate);
            health?.TakeDamage(poisonDamagePerTick);
            elapsed += poisonTickRate;
        }
    }
}