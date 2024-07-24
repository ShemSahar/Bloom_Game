using UnityEngine;

namespace MyGame
{
    public class RandomMovement : MonoBehaviour
    {
        public float speed = 5.0f;
        public LayerMask collisionLayer; // Layer to detect collisions for direction change
        public LayerMask wallAndFurnitureLayer; // Layer to detect walls and furniture
        public GameObject stopCollisionObject; // Game object that stops the vacuum when collided with

        private Vector3 direction;
        private Rigidbody rb;
        private Vector3 lastPosition;
        private float positionCheckDelay = 2.0f; // Time interval to check if the object is stuck
        private float timeSinceLastCheck = 0f;

        private AudioSource audioSource;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            ChangeDirection(); // Set initial random direction
        }

        void FixedUpdate()
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            CheckIfStuck();
        }

        private void ChangeDirection()
        {
            Vector3 randomDirection = Vector3.zero;
            bool validDirection = false;

            while (!validDirection)
            {
                float randomX = Random.Range(-1f, 1f);
                float randomZ = Random.Range(-1f, 1f);
                randomDirection = new Vector3(randomX, 0f, randomZ).normalized;

                Ray ray = new Ray(transform.position, randomDirection);
                if (!Physics.Raycast(ray, 1.0f, wallAndFurnitureLayer))
                {
                    validDirection = true;
                }
            }

            direction = randomDirection;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & collisionLayer) != 0)
            {
                ChangeDirection(); // Change direction on collision with specified layers
            }

            if (collision.gameObject == stopCollisionObject)
            {
                StopMovement(collision.gameObject);
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

        private void StopMovement(GameObject stopObject)
        {
            // Stop the vacuum's movement
            speed = 0f;

            // Turn off the audio component
            if (audioSource != null)
            {
                audioSource.enabled = false;
            }

            // Move the vacuum to the same position as the stop object
            transform.position = stopObject.transform.position;

            Debug.Log("Vacuum stopped and moved to the position of the specified object.");
        }
    }
}
