using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Settings")]
    public Transform spawnPoint;
    public float fallThreshold = -10f; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Checks if the player fell over threshold
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        // Teleport player to the spawn point
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player has respawned");
    }
}