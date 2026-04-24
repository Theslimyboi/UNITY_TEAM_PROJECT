using UnityEngine;

// Attach to enemy GameObjects to give them health
public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() { currentHealth = maxHealth; }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // If this is the player, tell the UI Manager
        if (gameObject.CompareTag("Player"))
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} defeated!");
        Destroy(gameObject);
    }
}
