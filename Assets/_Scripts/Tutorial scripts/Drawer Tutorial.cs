using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class DrawerTutorial : MonoBehaviour, IInteractable
    {
        [Header("Drawer Settings")]
        public float moveDistance = 1.5f;  // Distance to move the drawer along the Z axis
        public float moveSpeed = 1.0f;  // Speed at which the drawer moves

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Material outlineMaterial;  // Reference to the outline material
        public Color outlineColor = Color.blue;  // Color of the outline
        public float outlineWidth = 1.0f;  // Width of the outline

        private Vector3 initialPosition;
        private bool isOpen = false;
        private Material originalMaterial;
        private bool hasInteracted = false;
        public MissionManager missionManager;  // Reference to the MissionManager

        private void Start()
        {
            initialPosition = transform.position;

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (tutorialUI != null)
            {
                tutorialUI.SetActive(false);
            }

            if (outlineMaterial != null)
            {
                outlineMaterial.SetColor("_BaseColor", outlineColor);
                outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
            }

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterial = renderer.material;
            }
        }

        private void Update()
        {
            if (IsPlayerInRange())
            {
                ShowTutorialUI(true);
                EnableOutline(true);
            }
            else
            {
                ShowTutorialUI(false);
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
            StartCoroutine(MoveDrawer());
            hasInteracted = true;
            missionManager?.CompleteMission();  // Mark mission as completed
        }

        private IEnumerator MoveDrawer()
        {
            Vector3 targetPosition = isOpen ? initialPosition : initialPosition + new Vector3(0, 0, moveDistance);
            Vector3 startPosition = transform.position;

            float elapsedTime = 0f;
            while (elapsedTime < moveSpeed)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            isOpen = !isOpen;
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

        private void ShowTutorialUI(bool show)
        {
            if (tutorialUI != null)
            {
                tutorialUI.SetActive(show);
            }
        }

        private void EnableOutline(bool enable)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = enable ? outlineMaterial : originalMaterial;
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

        public void SetInteractable(bool isActive)
        {
            throw new System.NotImplementedException();
        }
    }
}
