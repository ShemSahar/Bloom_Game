using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class StoveDoor : MonoBehaviour, IInteractable
    {
        [Header("Rotation Settings")]
        public Transform doorTransform;  // Drag and drop the door component here
        public float rotationAngle = -90f;  // The angle to rotate
        public float rotationSpeed = 2f;  // Speed of the rotation

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        [Header("Gizmos Settings")]
        public Vector3 gizmosOffset = Vector3.zero;  // Offset for Gizmos location

        [Header("Animator Settings")]
        public Animator doorAnimator;  // Assign the Animator component in the Inspector
        public string openTrigger = "Open";  // Animator trigger for opening the door
        public string closeTrigger = "Close";  // Animator trigger for closing the door

        [Header("Outline Settings")]
        public Outline doorOutline;  // Assign the Outline component in the Inspector

        private bool isOpen = false;  // To keep track of the door's state
        private bool isAnimating = false;  // To prevent multiple interactions during animation
        private Quaternion openRotation;
        private Quaternion closedRotation;

        private void Start()
        {
            if (doorTransform == null)
            {
                Debug.LogError("Door Transform is not assigned.");
            }
            if (doorAnimator == null)
            {
                Debug.LogError("Door Animator is not assigned.");
            }
            if (doorOutline == null)
            {
                Debug.LogError("Door Outline is not assigned.");
            }
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
            }
            else
            {
                interactButton.onClick.AddListener(Interact);
            }

            closedRotation = doorTransform.rotation;
            openRotation = closedRotation * Quaternion.Euler(rotationAngle, 0, 0);
        }

        private void Update()
        {
            HandleOutline();
        }

        private void HandleOutline()
        {
            if (IsPlayerInRange() && !isAnimating)
            {
                doorOutline.enabled = true;
            }
            else
            {
                doorOutline.enabled = false;
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
            if (IsPlayerInRange() && !isAnimating)
            {
                StartCoroutine(RotateDoor());
            }
        }

        private IEnumerator RotateDoor()
        {
            isAnimating = true;

            if (doorAnimator != null)
            {
                if (isOpen)
                {
                    doorAnimator.SetTrigger(closeTrigger);
                }
                else
                {
                    doorAnimator.SetTrigger(openTrigger);
                }
            }
            else
            {
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
            }

            isOpen = !isOpen;
            isAnimating = false;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere to visualize interact range in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + gizmosOffset, interactRange);
        }
    }
}
