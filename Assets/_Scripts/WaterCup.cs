using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class WaterCup : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private float waterAmount = 10f; // The amount of water this cup adds to the player's total
        [SerializeField]
        private float interactRange = 2.0f; // Range within which the player can interact
        [SerializeField]
        private Transform playerTransform; // Assign the player object in the Inspector
        [SerializeField]
        private Button interactButton; // Assign the button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Child Settings")]
        public GameObject cylinder;  // Reference to the child Cylinder object

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string drinkAnimationTrigger = "Drink";  // Name of the trigger for the drink animation

        [Header("Attachment Settings")]
        public Transform temporaryParent;  // Reference to the temporary parent object
        public float attachmentDuration = 3.0f;  // Duration to stay attached

        [Header("Audio Settings")]
        public AudioSource interactSound;  // AudioSource for the interaction sound

        private bool isFull = true; // Initial state of the cup
        private bool waterAdded = false;
        private Vector3 startPosition;  // Store the start position of the water cup
        private Quaternion startRotation;  // Store the start rotation of the water cup

        private Rigidbody rb;
        private Collider col;

        private void Start()
        {
            // Ensure the interactButton is assigned and set up the listener
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
                return;
            }
            interactButton.onClick.AddListener(HandleInteraction);

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the WaterCup or its children.");
                }
            }
            outline.enabled = true;  // Start with the outline toggled on

            if (cylinder == null)
            {
                Debug.LogError("Cylinder object is not assigned.");
            }
            else
            {
                ToggleCylinder(true); // Ensure the cylinder is visible at start
            }

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody is not assigned.");
            }
            if (col == null)
            {
                Debug.LogError("Collider is not assigned.");
            }

            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        private void Update()
        {
            if (IsPlayerInRange())
            {
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
        }

        private void HandleInteraction()
        {
            if (IsPlayerInRange() && isFull)
            {
                Interact();
            }
        }

        private bool IsPlayerInRange()
        {
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                return distanceToPlayer <= interactRange;
            }
            return false;
        }

        public void Interact()
        {
            if (isFull && !waterAdded)
            {
                // Trigger the drink animation
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(drinkAnimationTrigger);
                    Debug.Log("Drink animation triggered.");
                }
                else
                {
                    Debug.LogError("Player Animator is not assigned.");
                    return;
                }

                // Play the interaction sound
                if (interactSound != null)
                {
                    interactSound.Play();
                }
                else
                {
                    Debug.LogError("Interact sound is not assigned.");
                }

                AddWaterToResourceManager();
                isFull = false;
                ToggleCylinder(false);
                waterAdded = true;  // Ensure water is added only once
                outline.enabled = false;  // Toggle off the outline after interaction

                // Start the attachment process
                StartCoroutine(TemporaryAttachment());
            }
        }

        private void AddWaterToResourceManager()
        {
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddWater(waterAmount);
                Debug.Log("Added Water: " + waterAmount);
            }
            else
            {
                Debug.LogError("ResourceManager not found in the scene.");
            }
        }

        private void ToggleCylinder(bool isVisible)
        {
            if (cylinder != null)
            {
                MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();
                if (cylinderRenderer != null)
                {
                    cylinderRenderer.enabled = isVisible;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position to visualize interact range in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(HandleInteraction);
            }
        }

        private IEnumerator TemporaryAttachment()
        {
            // Disable Rigidbody and Collider
            if (rb != null) rb.isKinematic = true;
            if (col != null) col.enabled = false;

            // Attach to the temporary parent
            transform.SetParent(temporaryParent);
            Debug.Log("Water cup attached to temporary parent.");

            // Wait for the specified duration
            yield return new WaitForSeconds(attachmentDuration);

            // Detach and return to start position
            transform.SetParent(null);
            transform.position = startPosition;
            transform.rotation = startRotation;

            // Enable Rigidbody and Collider
            if (rb != null) rb.isKinematic = false;
            if (col != null) col.enabled = true;

            Debug.Log("Water cup detached from temporary parent and returned to start position.");
        }
    }
}
