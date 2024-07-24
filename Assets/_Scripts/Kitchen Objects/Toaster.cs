using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class Toaster : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private float interactRange = 2.0f; // Range within which the player can interact
        [SerializeField]
        private Transform playerTransform; // Assign the player object in the Inspector
        [SerializeField]
        private Button interactButton; // Assign the button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Color Change Settings")]
        public Color targetColor = Color.red;  // Color to change to
        public float colorChangeDuration = 2.0f;  // Duration of the color change

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interact animation

        private bool hasInteracted = false;
        private Renderer toasterRenderer;
        private ResourceManager resourceManager;
        private Color initialColor;

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
                    Debug.LogError("No Outline component found on the Toaster or its children.");
                }
            }
            outline.enabled = true;  // Start with the outline toggled on

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            toasterRenderer = GetComponent<Renderer>();
            if (toasterRenderer == null)
            {
                Debug.LogError("No Renderer component found on the Toaster or its children.");
            }
            initialColor = toasterRenderer.material.color;

            resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager == null)
            {
                Debug.LogError("ResourceManager not found in the scene.");
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

        private void HandleInteraction()
        {
            if (IsPlayerInRange() && !hasInteracted)
            {
                Interact();
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
            else
            {
                Debug.LogError("Player Animator is not assigned.");
                return;
            }

            if (resourceManager != null)
            {
                resourceManager.AddWater(-10);  // Decrease water amount by 10
                Debug.Log("Decreased Water: -10");
            }

            StartCoroutine(ChangeColor());

            outline.enabled = false;  // Toggle off the outline after interaction
            hasInteracted = true;
        }

        private IEnumerator ChangeColor()
        {
            float elapsedTime = 0f;
            while (elapsedTime < colorChangeDuration)
            {
                toasterRenderer.material.color = Color.Lerp(initialColor, targetColor, elapsedTime / colorChangeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            toasterRenderer.material.color = targetColor;
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

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere to visualize interact range in the editor
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
    }
}
