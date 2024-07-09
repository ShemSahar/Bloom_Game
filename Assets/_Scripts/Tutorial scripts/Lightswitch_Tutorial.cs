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

        private Material originalMaterial;
        private bool sunlightAdded = false;
        private bool lightStateChanged = false;
        private bool isBlinking = true;

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

            originalMaterial = GetComponent<Renderer>().material;
            if (originalMaterial == null)
            {
                Debug.LogError("No material found on the LightSwitch or its children.");
            }

            StartCoroutine(BlinkEmission());
        }

        private void Update()
        {
            if (IsPlayerInRange())
            {
                EnableEmission(true);
            }
            else
            {
                EnableEmission(false);
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
                isBlinking = false;  // Stop blinking
                EnableEmission(false);  // Ensure emission is off
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

        private void EnableEmission(bool enable)
        {
            if (originalMaterial != null)
            {
                if (enable)
                {
                    originalMaterial.EnableKeyword("_EMISSION");
                    originalMaterial.SetColor("_EmissionColor", Color.blue * 1.5f);  // Adjust the color and intensity as needed
                }
                else
                {
                    originalMaterial.DisableKeyword("_EMISSION");
                }
            }
        }

        private IEnumerator BlinkEmission()
        {
            while (isBlinking)
            {
                EnableEmission(true);
                yield return new WaitForSeconds(0.5f);
                EnableEmission(false);
                yield return new WaitForSeconds(0.5f);
            }
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
