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

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Material outlineMaterial;  // Reference to the outline material
        public Color outlineColor = Color.blue;  // Color of the outline
        public float outlineWidth = 1.0f;  // Width of the outline

        private Material originalMaterial;
        private bool sunlightAdded = false;

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
            if (controlledLight != null)
            {
                controlledLight.enabled = !controlledLight.enabled;
                Debug.Log("Controlled Light " + (controlledLight.enabled ? "Enabled" : "Disabled"));

                if (!sunlightAdded && controlledLight.enabled)
                {
                    AddSunlightToResourceManager();
                    missionManager.CompleteMission();  // Notify mission manager of completion
                }
            }
        }

        private void AddSunlightToResourceManager()
        {
            if (!sunlightAdded)
            {
                ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
                if (resourceManager != null)
                {
                    resourceManager.AddSunlight(sunlightAmount);
                    Debug.Log("Added Sunlight: " + sunlightAmount);
                    sunlightAdded = true;  // Ensure sunlight is added only once
                }
                else
                {
                    Debug.LogError("ResourceManager not found in the scene.");
                }
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
    }
}
