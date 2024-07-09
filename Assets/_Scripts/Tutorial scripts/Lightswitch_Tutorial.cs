using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LightSwitchTutorial : MonoBehaviour, IInteractable
    {
        public Light controlledLight;  // Reference to the Light component
        public float sunlightAmount = 15f;  // Amount of sunlight to add when the light is turned on
        public MissionManager missionManager;  // Reference to the MissionManager

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector
        public Outline outline;  // Reference to the Outline component

        private bool sunlightAdded = false;
        private bool lightStateChanged = false;

        private void Start()
        {
            if (controlledLight == null)
            {
                Debug.LogError("No Light component found on the LightSwitch or its children.");
            }

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the LightSwitch or its children.");
                }
            }
            outline.enabled = true;  // Start with the outline toggled on
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
            if (controlledLight != null && !lightStateChanged)
            {
                controlledLight.enabled = !controlledLight.enabled;
                Debug.Log("Controlled Light " + (controlledLight.enabled ? "Enabled" : "Disabled"));

                if (controlledLight.enabled && !sunlightAdded)
                {
                    AddSunlightToResourceManager();
                    missionManager.CompleteMission();  // Mark mission as completed
                    sunlightAdded = true;
                }
                lightStateChanged = true;
                outline.enabled = false;  // Toggle off the outline
                Invoke(nameof(ResetLightStateChanged), 0.1f); // Reset the state after a short delay to prevent double interaction
            }
        }

        private void ResetLightStateChanged()
        {
            lightStateChanged = false;
        }

        private void AddSunlightToResourceManager()
        {
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddSunlight(sunlightAmount);
                Debug.Log("Added Sunlight: " + sunlightAmount);
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

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(OnInteractButtonClicked);
            }
        }
    }
}
