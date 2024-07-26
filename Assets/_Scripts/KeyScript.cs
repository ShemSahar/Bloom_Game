using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class KeyScript : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector
        public GameObject keyIconUI;  // Reference to the key icon in the UI

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Mission Settings")]
        public MissionManager missionManager;  // Reference to the MissionManager

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
                Debug.LogError("Player Animator is not assigned in the Key script.");
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the Key or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off
        }

        private void Update()
        {
            if (!isCollected && IsPlayerInRange())
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

                // Inform the mission manager
                if (missionManager != null)
                {
                    missionManager.CompleteMission();
                }
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
