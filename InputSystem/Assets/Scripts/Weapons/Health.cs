using UnityEngine;

// Attach to enemies. For the player, damage goes through PlayerHealth instead.
public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() { currentHealth = maxHealth; }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        // Enemies just get destroyed. Player death is handled by PlayerHealth.
        if (!gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}