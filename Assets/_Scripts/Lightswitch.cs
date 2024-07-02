using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        public Light controlledLight;  // Reference to the Light component
        public float sunlightAmount = 10f;  // Amount of sunlight to add when the light is turned on

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        private void Start()
        {
            if (controlledLight == null)
            {
                Debug.LogError("No Light component found on the LightSwitch or its children.");
            }

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(TryInteract);
            }
        }

        private void TryInteract()
        {
            if (IsPlayerInRange())
            {
                Interact();
            }
        }

        public void Interact()
        {
            if (controlledLight != null)
            {
                controlledLight.enabled = !controlledLight.enabled;
                if (controlledLight.enabled)
                {
                    AddSunlightToResourceManager();
                }
            }
        }

        private void AddSunlightToResourceManager()
        {
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddSunlight(sunlightAmount);
            }
            else
            {
                Debug.LogError("ResourceManager not found in the scene.");
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
            // Draw a yellow sphere at the transform's position to visualize interact range in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(TryInteract);
            }
        }
    }
}
