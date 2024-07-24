using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LockedDoorScript : MonoBehaviour, IInteractable
    {
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector
        public float interactRange = 2.0f;  // Range within which the player can interact
        public GameObject lockedMessageUI;  // UI element to show "locked" message

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        private void Start()
        {
            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (lockedMessageUI != null)
            {
                lockedMessageUI.SetActive(false);
            }

            // Check if playerAnimator is assigned
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned in the LockedDoorScript.");
            }
        }

        private void Update()
        {
            // Outline removed, no additional logic needed here
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
            // Trigger the interact animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(interactAnimationTrigger);
                Debug.Log("Interact animation triggered.");
            }

            ShowLockedMessage();
        }

        private void ShowLockedMessage()
        {
            if (lockedMessageUI != null)
            {
                lockedMessageUI.SetActive(true);
                StartCoroutine(HideLockedMessageAfterDelay(4.0f));  // Hide after 4 seconds
                Debug.Log("Door is locked.");
            }
        }

        private IEnumerator HideLockedMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (lockedMessageUI != null)
            {
                lockedMessageUI.SetActive(false);
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