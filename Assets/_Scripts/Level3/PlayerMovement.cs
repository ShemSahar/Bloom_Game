using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform player1Transform;
    [SerializeField] private BallScript ballScript;
    private Vector2 touchPosition;
    private bool gameStarted = false;

    private void Start()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            gameStarted = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ballScript.StartBall();
        }

        if (gameStarted && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.y = Mathf.Clamp(touchPosition.y, -4f, 4f);  // Adjust as per your UI boundaries
            touchPosition.x = player1Transform.position.x;  // Keep the paddle only moving vertically
        }
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            rb.MovePosition(new Vector2(rb.position.x, touchPosition.y));
        }
    }
}
