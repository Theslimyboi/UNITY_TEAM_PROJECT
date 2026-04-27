using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
            return;
        }
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}


