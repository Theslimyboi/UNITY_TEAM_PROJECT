using UnityEngine;

public enum StatusEffect { Burning, Frozen, Poisoned, Stunned }



// Attach to enemies to handle status effect logic
public class StatusEffectController : MonoBehaviour
{
    public void ApplyEffect(StatusEffect effect, float duration)
    {
        Debug.Log($"{gameObject.name}: {effect} applied for {duration}s");
        // Add effect logic here (burning tick, freeze slow, etc.)
    }
}