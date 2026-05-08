using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private Animator animator;
    public int maxLives = 3;
    public int currentLives;
    private bool isDead = false;

    void Start()
    {
        // FIX: always reset to maxLives at start, regardless of serialized value
        currentLives = maxLives;
        isDead = false;
        animator = GetComponent<Animator>();
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

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        gameObject.SetActive(true);
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        StartCoroutine(DieAfterAnimation());
    }

    IEnumerator DieAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f);

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();

        gameObject.SetActive(false);
    }
}