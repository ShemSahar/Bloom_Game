using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class New_Key : MonoBehaviour, IInteractable
    {
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector
        public float interactRange = 2.0f;  // Range within which the player can interact
        public GameObject keyIconUI;  // Reference to the key icon in the UI

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        private bool isCollected = false;
        private Renderer keyRenderer;

        private void Start()
        {
            keyRenderer = GetComponent<Renderer>();

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (keyIconUI != null)
            {
                keyIconUI.SetActive(false);
            }

            // Check if playerAnimator is assigned
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned in the New_Key script.");
            }
        }

        private void Update()
        {
            if (IsPlayerInRange() && !isCollected)
            {
                EnableOutline(true);
            }
            else
            {
                EnableOutline(false);
            }
        }

        private void OnInteractButtonClicked()
        {
            if (IsPlayerInRange())
            {
                Interact();
            }
        }

        public void Interact()
        {
            if (!isCollected)
            {
                // Trigger the interact animation
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(interactAnimationTrigger);
                    Debug.Log("Interact animation triggered.");
                }

                isCollected = true;
                keyRenderer.enabled = false;

                if (keyIconUI != null)
                {
                    keyIconUI.SetActive(true);
                }

                Debug.Log("Key collected!");
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

        private void EnableOutline(bool enable)
        {
            Outline outline = GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = enable;
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
