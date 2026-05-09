using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 5f;

    public bool moveVertical = false;

    private Vector3 startPos;
    private Rigidbody2D rb;
    private Vector2 platformVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        float movement = Mathf.Sin(Time.time * speed) * distance;
        Vector2 nextPosition;

        if (moveVertical)
        {
            nextPosition = new Vector2(startPos.x, startPos.y + movement);
        }
        else
        {
            nextPosition = new Vector2(startPos.x + movement, startPos.y);
        }

        platformVelocity = (nextPosition - rb.position) / Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                if (collision.GetContact(0).normal.y < -0.5f)
                {
                    if (!moveVertical)
                    {
                        playerRb.AddForce(new Vector2(platformVelocity.x * 2f, 0));
                    }
                }
            }
        }
    }
}