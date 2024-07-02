using UnityEngine;

namespace MyGame
{
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        public Light controlledLight;  // Reference to the Light component
        public float sunlightAmount = 10f;  // Amount of sunlight to add when the light is turned on

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector

        private void Start()
        {
            if (controlledLight == null)
            {
                Debug.LogError("No Light component found on the LightSwitch or its children.");
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
    }
}
