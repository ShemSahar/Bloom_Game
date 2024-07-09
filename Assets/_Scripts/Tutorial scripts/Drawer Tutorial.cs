using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class DrawerTutorial : MonoBehaviour, IInteractable
    {
        [Header("Drawer Settings")]
        public Transform drawerTransform;  // Assign the target drawer object in the Inspector
        public float moveDistance = 1.5f;  // Distance to move the drawer along the Z axis
        public float moveSpeed = 1.0f;  // Speed at which the drawer moves

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        private Vector3 initialPosition;
        private bool isOpen = false;
        private bool hasInteracted = false;
        public MissionManager missionManager;  // Reference to the MissionManager

        private void Start()
        {
            if (drawerTransform == null)
            {
                Debug.LogError("DrawerTransform is not assigned.");
                return;
            }

            initialPosition = drawerTransform.position;

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (tutorialUI != null)
            {
                tutorialUI.SetActive(false);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the Drawer or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off
        }

        private void Update()
        {
            if (IsPlayerInRange())
            {
                ShowTutorialUI(true);
                outline.enabled = true;
            }
            else
            {
                ShowTutorialUI(false);
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
            if (!hasInteracted)
            {
                StartCoroutine(MoveDrawer());
                missionManager?.CompleteMission();  // Mark mission as completed
                outline.enabled = false;  // Toggle off the outline after interaction
                hasInteracted = true;
            }
            else
            {
                StartCoroutine(MoveDrawer());
            }
        }

        private IEnumerator MoveDrawer()
        {
            Vector3 targetPosition = isOpen ? initialPosition : initialPosition + new Vector3(0, 0, moveDistance);
            Vector3 startPosition = drawerTransform.position;

            float elapsedTime = 0f;
            while (elapsedTime < moveSpeed)
            {
                drawerTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            drawerTransform.position = targetPosition;
            isOpen = !isOpen;
        }

        public bool IsDrawerOpen()
        {
            return isOpen;
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
