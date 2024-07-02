using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    [Header("Wall Check")]
    public LayerMask whatIsWall;
    public float wallCheckDistance;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;

    public bool WallLeft { get; private set; }
    public bool WallRight { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForWall();
    }

    private void CheckForWall()
    {
        WallRight = Physics.Raycast(transform.position, orientation.right, wallCheckDistance, whatIsWall);
        WallLeft = Physics.Raycast(transform.position, -orientation.right, wallCheckDistance, whatIsWall);
    }

    // Method to adjust the velocity based on wall detection
    public void AdjustVelocity()
    {
        if ((WallRight && rb.velocity.x > 0) || (WallLeft && rb.velocity.x < 0))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
    }
}
