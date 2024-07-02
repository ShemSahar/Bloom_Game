using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class JoystickController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _doubleJumpForce;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _furnitureLayer;

    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float interactRange = 2.0f;
    [SerializeField] private Button interactButton; // Reference to the interact button

    [Header("Additional Settings")]
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airMultiplier = 0.5f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float smoothTime = 0.1f; // Smoothing time for transitions

    private bool _isGrounded;
    private bool _canDoubleJump;
    private bool _readyToJump = true;

    private float _currentMoveSpeed;
    private float _moveMagnitude;
    private float _moveMagnitudeVelocity;

    private void Awake()
    {
        // Ensure the Rigidbody constraints are set correctly at the start
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // Link the interact button to the Interact method
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(Interact);
        }
    }

    private void Update()
    {
        // Check for interaction input from keyboard
        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }

        // Apply ground drag when grounded
        if (IsGrounded())
        {
            _rigidbody.drag = groundDrag;
        }
        else
        {
            _rigidbody.drag = 0;
        }

        // Update the animator with the smoothed movement magnitude
        float targetMagnitude = new Vector2(_joystick.Horizontal, _joystick.Vertical).magnitude;
        _moveMagnitude = Mathf.SmoothDamp(_moveMagnitude, targetMagnitude, ref _moveMagnitudeVelocity, smoothTime);
        _animator.SetFloat("MoveMagnitude", _moveMagnitude);

        // Update the grounded status in the Animator
        _animator.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        // Set the constraints again to ensure they remain active
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // Apply movement with air multiplier if in the air
        float moveSpeed = IsGrounded() ? _currentMoveSpeed : _currentMoveSpeed * airMultiplier;
        _rigidbody.velocity = new Vector3(_joystick.Horizontal * moveSpeed, _rigidbody.velocity.y, _joystick.Vertical * moveSpeed);

        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            Vector3 direction = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            if (direction != Vector3.zero) // Prevent zero vector normalization error
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void Jump()
    {
        if (_readyToJump && IsGrounded())
        {
            StartCoroutine(JumpCooldown());
            Jump(_jumpForce);
            _canDoubleJump = true;
            _animator.SetTrigger("FirstJump"); // Trigger first jump animation
        }
        else if (_canDoubleJump)
        {
            DoubleJump();
            _animator.SetTrigger("DoubleJump"); // Trigger double jump animation
        }
    }

    private IEnumerator JumpCooldown()
    {
        _readyToJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        _readyToJump = true;
    }

    public void Jump(float force)
    {
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        _rigidbody.AddForce(transform.up * force, ForceMode.Impulse);
    }

    public void DoubleJump()
    {
        _canDoubleJump = false;
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        _rigidbody.AddForce(transform.up * _doubleJumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        // Simple ground check using a raycast
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, _groundLayer);
    }

    private bool IsOnFurniture()
    {
        // Check if the player is on furniture using a raycast
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, _furnitureLayer);
    }

    public void Interact()
    {
        // Find nearby interactable objects
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, interactRange);
        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                _animator.SetTrigger("Interact"); // Trigger interact animation
                break; // Only interact with one object at a time
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the player's position to visualize interact range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, interactRange);
    }

    public void SetSpeed(float newSpeed)
    {
        _currentMoveSpeed = newSpeed;
    }
}
