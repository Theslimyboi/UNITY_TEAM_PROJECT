using UnityEngine;

// Attach to the SlashEffect prefab
// Spawned by MeleeWeapon on each attack
public class MeleeSlashEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.2f;
    [SerializeField] private float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Fade out over time
        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, 0f, fadeSpeed * Time.deltaTime);
        sr.color = c;
    }
}