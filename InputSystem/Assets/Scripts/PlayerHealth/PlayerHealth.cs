using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;
    private bool isDead = false;

    void Start()
    {
        // FIX: always reset to maxLives at start, regardless of serialized value
        currentLives = maxLives;
        isDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentLives -= amount;

        if (currentLives <= 0)
            Die();
    }

    // FIX: added Revive() so PlayerRespawn or future systems can bring the player back
    public void Revive()
    {
        isDead = false;
        currentLives = maxLives;
        gameObject.SetActive(true);
    }

    void Die()
    {
        isDead = true;
        gameObject.SetActive(false);
        GameManager.Instance.PlayerDied();
    }
}