using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyGame
{
    public class DoorWithPasscodeScript : MonoBehaviour, IInteractable
    {
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector
        public float interactRange = 2.0f;  // Range within which the player can interact
        public GameObject lockedMessageUI;  // UI element to show "locked" message
        public NumpadScript numpadScript;  // Reference to the NumpadScript

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Audio Settings")]
        public AudioClip interactSound;  // Audio clip for the interaction sound

        private bool isUnlocked = false;

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

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the Door or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off

            // Check if playerAnimator is assigned
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned in the DoorScript.");
            }

            if (interactSound == null)
            {
                Debug.LogError("Interact Sound is not assigned.");
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

        public void Interact()
        {
            // Trigger the interact animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(interactAnimationTrigger);
                Debug.Log("Interact animation triggered.");
            }

            if (isUnlocked)
            {
                UnlockDoor();
            }
            else
            {
                ShowLockedMessage();
            }
        }

        public void UnlockDoor()
        {
            Debug.Log("Door unlocked! Loading next scene...");
            if (interactSound != null)
            {
                PersistentAudioManager.Instance.PlaySound(interactSound, true);
            }
            SceneManager.LoadScene("Loading2");
        }

        private void ShowLockedMessage()
        {
            if (lockedMessageUI != null)
            {
                lockedMessageUI.SetActive(true);
                StartCoroutine(HideLockedMessageAfterDelay(4.0f));  // Hide after 4 seconds
                Debug.Log("Door is locked. Enter the correct passcode.");
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
