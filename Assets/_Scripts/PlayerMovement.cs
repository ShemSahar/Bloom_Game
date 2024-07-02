using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float groundDrag;
        public float jumpForce;
        public float doubleJumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        private bool readyToJump;
        private bool canDoubleJump = true;

        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode interactKey = KeyCode.E;  // Key to interact with objects

        [Header("Ground Check")]
        public float playerHeight;
        public LayerMask whatIsGround;  // Set this in the Unity Inspector
        public LayerMask whatIsFurniture;  // Set this in the Unity Inspector
        private bool grounded;

        [Header("Slope Handling")]
        public float maxSlopeAngle;
        private RaycastHit slopeHit;
        private bool exitingSlope;

        public Transform orientation;

        [Header("Interaction")]
        public float interactRange;  // Range within which the player can interact

        private float horizontalInput;
        private float verticalInput;
        private Vector3 moveDirection;
        private Rigidbody rb;

        private ResourceManager resourceManager;
        private Vector3 lastPosition;  // Track the last position to measure movement

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            readyToJump = true;
            resourceManager = FindObjectOfType<ResourceManager>();
            lastPosition = transform.position;  // Initialize lastPosition
        }

        private void Update()
        {
            int combinedLayerMask = whatIsGround | whatIsFurniture;
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.9f, combinedLayerMask);
            Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.9f), grounded ? Color.green : Color.red);

            MyInput();
            SpeedControl();

            if (grounded)
            {
                rb.drag = groundDrag;
                canDoubleJump = true;  // Reset double jump when grounded
            }
            else
                rb.drag = 0;

            HandleInteraction();        }

        private void FixedUpdate()
        {
            MovePlayer();
        }


        private void MyInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(jumpKey))
            {
                if (grounded && readyToJump)
                {
                    readyToJump = false;
                    Jump(jumpForce);
                    Invoke(nameof(ResetJump), jumpCooldown);
                }
                else if (canDoubleJump)
                {
                    DoubleJump();
                }
            }
        }


        private void DoubleJump()
        {
            canDoubleJump = false;  // Disable further double jumps until grounded again
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset y velocity
            rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
        }

        private void Jump(float force)
        {
            exitingSlope = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset y velocity
            rb.AddForce(transform.up * force, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            readyToJump = true;
            exitingSlope = false;
        }

        private void MovePlayer()
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }

            rb.useGravity = !OnSlope();

            RotatePlayer();  // Add this line to call the rotation method
        }

        private void RotatePlayer()
        {
            Vector3 flatMoveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
            if (flatMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
            }
        }

        private void SpeedControl()
        {
            // This function could be used to limit the player's speed
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        private void HandleInteraction()
        {
            // This function could check for interaction opportunities in proximity
            if (Input.GetKeyDown(interactKey))
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.TryGetComponent<IInteractable>(out IInteractable interactable))
                    {
                        interactable.Interact();
                        break;
                    }
                }
            }
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.9f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }
            return false;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        }
    }
}
