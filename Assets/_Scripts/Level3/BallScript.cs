using UnityEngine;

public class BallScript : MonoBehaviour
{
    public static BallScript instance;
    private Rigidbody2D rb;
    public float speed = 5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the Ball.");
        }
        rb.velocity = Vector2.zero;
    }

    public void StartBall()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.right * speed;
        }
        else
        {
            Debug.LogError("Rigidbody2D is null in StartBall.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 velocity = rb.velocity;
            velocity.y = (rb.position.y - collision.transform.position.y) * 2f;  // Adjust as needed
            rb.velocity = velocity.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            if (other.gameObject.name == "Player1Goal")
            {
                GameManager.instance.AddScore(2);
            }
            else if (other.gameObject.name == "Player2Goal")
            {
                GameManager.instance.AddScore(1);
            }
            ResetBall();
        }
    }

    private void ResetBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        GameManager.instance.StartGame();
    }
}
