using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private Animator animator;
    public int maxLives = 3;
    public int currentLives;
    private bool isDead = false;

    [Header("Audio")]
    public AudioSource audioSource;          // <-- pridėta
    public AudioClip playerHurtClip;         // <-- pridėta

    void Start()
    {
        currentLives = maxLives;
        isDead = false;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentLives -= amount;

        // --- ŽAIDĖJO SUŽALOJIMO GARSAS ---
        if (audioSource != null && playerHurtClip != null)
            audioSource.PlayOneShot(playerHurtClip);

        if (currentLives <= 0)
            Die();
    }

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
