using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class Remote : MonoBehaviour, IInteractable
    {
        public Light controlledLight;  // Reference to the Light component
        private bool hasInteractedAutomatically = false;  // Flag to track automatic interaction

        [SerializeField]
        private LayerMask interactLayer;  // Layer that triggers automatic interaction

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Vacuum Settings")]
        public LayerMask vacuumLayer;  // Layer that the vacuum is on

        private Rigidbody rb;

        private void Start()
        {
            // Check for the Light component
            if (controlledLight == null)
            {
                controlledLight = GetComponentInChildren<Light>();
                if (controlledLight == null)
                {
                    Debug.LogError("No Light component found on the Remote or its children.");
                }
            }

            // Ensure a BoxCollider is added and set as non-trigger for physical blocking
            BoxCollider collider = gameObject.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider>();
                collider.isTrigger = false;  // This will now physically block the object
            }
            else
            {
                collider.isTrigger = false;  // Ensure it is not a trigger
            }

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the Remote or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }
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

        private void OnInteractButtonClicked()
        {
            if (IsPlayerInRange())
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

        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collision is with the specified interaction layer and control hasn't been taken over manually
            if (((1 << collision.gameObject.layer) & interactLayer) != 0 && !hasInteractedAutomatically)
            {
                // Turn on the light if it's the first interaction and hasn't been turned on manually
                if (controlledLight != null && !controlledLight.enabled)
                {
                    controlledLight.enabled = true;
                    hasInteractedAutomatically = true;  // Ensure this happens only once
                }
            }

            // Check if the collision is with the vacuum layer
            if (((1 << collision.gameObject.layer) & vacuumLayer) != 0)
            {
                // Freeze position on X, Y, and Z
                FreezePosition();
            }
        }

        public void Interact()
        {
            // Trigger the interact animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(interactAnimationTrigger);
                Debug.Log("Interact animation triggered.");
            }

            // Toggle the light on and off manually after the first automatic interaction
            if (controlledLight != null)
            {
                controlledLight.enabled = !controlledLight.enabled;
            }
            else
            {
                Debug.LogError("Controlled light not assigned or found.");
            }
        }

        private void FreezePosition()
        {
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(OnInteractButtonClicked);
            }
        }
    }
}
