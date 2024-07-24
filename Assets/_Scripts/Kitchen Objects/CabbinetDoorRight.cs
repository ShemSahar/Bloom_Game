using System.Collections;
using UnityEngine;

namespace MyGame
{
    public class RightCabinetDoor : MonoBehaviour, IInteractable
    {
        [Header("Rotation Settings")]
        public Transform doorTransform;  // Drag and drop the door component here
        public float rotationAngle = -90f;  // The angle to rotate
        public float rotationSpeed = 2f;  // Speed of the rotation
        public KeyCode interactKey = KeyCode.E;  // Key to interact with the door

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector

        [Header("Gizmos Settings")]
        public Vector3 gizmosOffset = Vector3.zero;  // Offset for Gizmos location

        private bool isOpen = false;  // To keep track of the door's state
        private bool isAnimating = false;  // To prevent multiple interactions during animation

        private Quaternion closedRotation;
        private Quaternion openRotation;

        private void Start()
        {
            // Store the initial and target rotations
            if (doorTransform != null)
            {
                closedRotation = doorTransform.rotation;
                openRotation = closedRotation * Quaternion.Euler(0, rotationAngle, 0);
            }
            else
            {
                Debug.LogError("Door Transform is not assigned.");
            }
        }

        private void Update()
        {
            HandleInteraction();
        }

        private void HandleInteraction()
        {
            if (Input.GetKeyDown(interactKey) && IsPlayerInRange() && !isAnimating)
            {
                StartCoroutine(RotateDoor());
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
            // Interact method can be used if called externally
            if (IsPlayerInRange() && !isAnimating)
            {
                StartCoroutine(RotateDoor());
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

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere to visualize interact range in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + gizmosOffset, interactRange);
        }

        public void SetInteractable(bool isActive)
        {
            throw new System.NotImplementedException();
        }
    }
}
