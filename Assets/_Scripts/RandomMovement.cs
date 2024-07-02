using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public LayerMask collisionLayer; // Layer to detect collisions for direction change

    private Vector3 direction;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private float positionCheckDelay = 2.0f; // Time interval to check if the object is stuck
    private float timeSinceLastCheck = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChangeDirection(); // Set initial random direction
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        CheckIfStuck();
    }

    private void ChangeDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        direction = new Vector3(randomX, 0f, randomZ).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayer) != 0)
        {
            ChangeDirection(); // Change direction on collision with specified layers
        }
    }

    private void CheckIfStuck()
    {
        // Check periodically if the object is stuck by comparing its current and last positions
        timeSinceLastCheck += Time.fixedDeltaTime;
        if (timeSinceLastCheck >= positionCheckDelay)
        {
            if (Vector3.Distance(rb.position, lastPosition) < 0.01f) // Very small movement suggests being stuck
            {
                ChangeDirection(); // Change direction to possibly get unstuck
            }
            lastPosition = rb.position;
            timeSinceLastCheck = 0;
        }
    }
}
