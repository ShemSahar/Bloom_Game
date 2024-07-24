using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class CabinetDoor : MonoBehaviour, IInteractable
    {
        [Header("Rotation Settings")]
        public Transform doorTransform;  // Drag and drop the door component here
        public float rotationAngle = 90f;  // The angle to rotate
        public float rotationSpeed = 2f;  // Speed of the rotation

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Gizmos Settings")]
        public Vector3 gizmosOffset = Vector3.zero;  // Offset for Gizmos location

        private bool isOpen = false;  // To keep track of the door's state
        private bool isAnimating = false;  // To prevent multiple interactions during animation

        private Quaternion closedRotation;
        private Quaternion openRotation;

        private void Start()
        {
            // Ensure the doorTransform is assigned
            if (doorTransform == null)
            {
                Debug.LogError("Door Transform is not assigned.");
                return;
            }

            // Store the initial and target rotations
            closedRotation = doorTransform.rotation;
            openRotation = closedRotation * Quaternion.Euler(0, rotationAngle, 0);

            // Ensure the interactButton is assigned and set up the listener
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
                return;
            }
            interactButton.onClick.AddListener(HandleInteraction);

            // Ensure the outline is assigned
            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the CabinetDoor or its children.");
                }
            }
            outline.enabled = true;  // Start with the outline toggled on

            // Check if playerAnimator is assigned
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned in the CabinetDoor script.");
            }
        }

        private void Update()
        {
            if (IsPlayerInRange() && !isAnimating)
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
            if (IsPlayerInRange())
            {
                Interact();
            }
        }

        public void Interact()
        {
            if (!isAnimating)
            {
                // Trigger the interact animation
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(interactAnimationTrigger);
                    Debug.Log("Interact animation triggered.");
                }

                StartCoroutine(RotateDoor());
                outline.enabled = false;  // Toggle off the outline after interaction
            }
        }

        private IEnumerator RotateDoor()
        {
            isAnimating = true;

            Quaternion targetRotation = isOpen ? closedRotation : openRotation;
            Quaternion startRotation = doorTransform.rotation;

            float time = 0f;

            while (time < 1f)
            {
                doorTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, time);
                time += Time.deltaTime * rotationSpeed;
                yield return null;
            }

            doorTransform.rotation = targetRotation;
            isOpen = !isOpen;
            isAnimating = false;
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
            // Draw a yellow sphere to visualize interact range in the editor with offset
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + gizmosOffset, interactRange);
        }

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(HandleInteraction);
            }
        }

        public void SetInteractable(bool isActive)
        {
            throw new System.NotImplementedException();
        }
    }
}
