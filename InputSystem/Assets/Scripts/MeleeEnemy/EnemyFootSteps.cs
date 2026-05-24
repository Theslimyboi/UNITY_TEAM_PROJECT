using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyFootsteps : MonoBehaviour
{
    public AudioSource walkAudio;
    public float movementThreshold = 0.1f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (walkAudio == null)
            walkAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (rb == null || walkAudio == null) return;

        // Jei priešas juda
        if (Mathf.Abs(rb.linearVelocity.x) > movementThreshold)
        {
            if (!walkAudio.isPlaying)
                walkAudio.Play();
        }
        else
        {
            if (walkAudio.isPlaying)
                walkAudio.Stop();
        }
    }
}
