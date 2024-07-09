using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class Key : MonoBehaviour, IInteractable
    {
        public DrawerTutorial drawerScript;  // Reference to the DrawerTutorial script
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector
        public float interactRange = 2.0f;  // Range within which the player can interact
        public GameObject keyIconUI;  // Reference to the key icon in the UI

        private bool isCollected = false;
        private Renderer keyRenderer;
        private Outline outline;

        private void Start()
        {
            keyRenderer = GetComponent<Renderer>();
            outline = GetComponent<Outline>();

            if (outline == null)
            {
                Debug.LogError("No Outline component found on the Key or its children.");
            }
            outline.enabled = false;  // Start with the outline toggled off

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (keyIconUI != null)
            {
                keyIconUI.SetActive(false);
            }
        }

        private void Update()
        {
            if (drawerScript.IsDrawerOpen() && IsPlayerInRange() && !isCollected)
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
            if (drawerScript.IsDrawerOpen() && IsPlayerInRange())
            {
                Interact();
            }
        }

        public void Interact()
        {
            if (!isCollected)
            {
                isCollected = true;
                keyRenderer.enabled = false;

                if (keyIconUI != null)
                {
                    keyIconUI.SetActive(true);
                }

                outline.enabled = false;  // Toggle off the outline after interaction

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
