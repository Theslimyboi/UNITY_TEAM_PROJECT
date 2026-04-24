using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;

    private bool isDead = false;

    void Start()
    {
        currentLives = maxLives;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentLives -= amount;

        if (currentLives <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Paslepiame žaidėją
        gameObject.SetActive(false);

        // Pranešame GameManager
        GameManager.Instance.PlayerDied();
    }
}

