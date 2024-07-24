using System.Collections;
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
        public Outline outline;  // Reference to the Outline component

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        private bool sunlightAdded = false;  // Ensure sunlight is only added once
        private bool lightStateChanged = false;

        private void Start()
        {
            if (controlledLight == null)
            {
                Debug.LogError("No Light component found on the LightSwitch or its children.");
            }
            else
            {
                controlledLight.intensity = 0.05f;  // Start with low intensity
                controlledLight.enabled = true;  // Ensure the light is always on
            }

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(TryInteract);
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

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }
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

        private void TryInteract()
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
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(interactAnimationTrigger);
                    Debug.Log("Interact animation triggered.");
                }
                else
                {
                    Debug.LogError("Player Animator is not assigned.");
                    return;
                }

                if (controlledLight.intensity == 0.05f)
                {
                    controlledLight.intensity = 1.0f;
                    if (!sunlightAdded)
                    {
                        AddSunlightToResourceManager();
                        sunlightAdded = true;
                    }
                }
                else
                {
                    controlledLight.intensity = 0.05f;
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
