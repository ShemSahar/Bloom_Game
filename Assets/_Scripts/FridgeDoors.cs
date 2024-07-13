using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Make sure to include this namespace

namespace MyGame
{
    public class CabinetDoorWithLight : MonoBehaviour, IInteractable
    {
        [Header("Rotation Settings")]
        public Transform doorTransform;  // Drag and drop the door component here
        public float rotationAngle = 90f;  // The angle to rotate
        public float rotationSpeed = 2f;  // Speed of the rotation
        public Button interactButton;  // Assign the button in the Inspector

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector

        [Header("Light Settings")]
        public Light cabinetLight;  // Drag and drop the light component here

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

            // Ensure the cabinetLight is assigned
            if (cabinetLight == null)
            {
                Debug.LogError("Cabinet Light is not assigned.");
                return;
            }

            // Store the initial and target rotations
            closedRotation = doorTransform.rotation;
            openRotation = closedRotation * Quaternion.Euler(0, rotationAngle, 0);

            // Ensure the light is initially off
            cabinetLight.enabled = false;

            // Ensure the interactButton is assigned and set up the listener
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
                return;
            }
            interactButton.onClick.AddListener(HandleInteraction);
        }

        private void HandleInteraction()
        {
            if (IsPlayerInRange() && !isAnimating)
            {
                StartCoroutine(RotateDoorAndToggleLight());
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
                StartCoroutine(RotateDoorAndToggleLight());
            }
        }

        private IEnumerator RotateDoorAndToggleLight()
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

            // Toggle the light state
            cabinetLight.enabled = isOpen;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere to visualize interact range in the editor with offset
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + gizmosOffset, interactRange);
        }
    }
}
