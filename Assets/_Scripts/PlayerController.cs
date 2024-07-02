using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 360;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;
    private bool canDoubleJump = true;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsFurniture;
    private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Interaction")]
    [SerializeField] private float interactRange;

    private Vector3 _input;
    private Vector3 moveDirection;
    private ResourceManager resourceManager;
    private Vector3 lastPosition;

    private void Awake()
    {
        // Freeze rotation on X and Z axes to keep the player upright
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        readyToJump = true;
        resourceManager = FindObjectOfType<ResourceManager>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        int combinedLayerMask = whatIsGround | whatIsFurniture;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.9f, combinedLayerMask);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.9f), grounded ? Color.green : Color.red);

        GatherInput();
        Look();
        SpeedControl();

        if (grounded)
        {
            _rb.drag = groundDrag;
            canDoubleJump = true;
        }
        else
        {
            _rb.drag = 0;
        }

        HandleInteraction();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

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

    void Look()
    {
        if (_input != Vector3.zero)
        {
            var relative = (transform.position + _input) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        moveDirection = new Vector3(_input.x, 0, _input.z).normalized;

        if (OnSlope() && !exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection() * _speed * 20f, ForceMode.Force);

            if (_rb.velocity.y > 0)
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            _rb.AddForce(moveDirection * _speed * 10f, ForceMode.Force);
        }
        else
        {
            _rb.AddForce(moveDirection * _speed * 10f * airMultiplier, ForceMode.Force);
        }

        _rb.useGravity = !OnSlope();
    }

    void Jump(float force)
    {
        exitingSlope = true;
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * force, ForceMode.Impulse);
    }

    void DoubleJump()
    {
        canDoubleJump = false;
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }



    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (flatVel.magnitude > _speed)
        {
            Vector3 limitedVelocity = flatVel.normalized * _speed;
            _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
        }
    }

    void HandleInteraction()
    {
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

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.9f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
}
