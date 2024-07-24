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

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Audio Settings")]
        public AudioSource interactSoundOn;  // Reference to the AudioSource component for the On sound
        public AudioSource interactSoundOff;  // Reference to the AudioSource component for the Off sound

        private bool sunlightAdded = false;
        private bool lightStateChanged = false;

        private void Start()
        {
            if (controlledLight == null)
            {
                Debug.LogError("No Light component found on the LightSwitch or its children.");
            }
            else
            {
                controlledLight.intensity = 0.05f;  // Set initial low intensity
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

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            if (interactSoundOn == null)
            {
                Debug.LogError("Interact Sound On is not assigned.");
            }

            if (interactSoundOff == null)
            {
                Debug.LogError("Interact Sound Off is not assigned.");
            }
        }

        private void Update()
        {
            // You can add any additional update logic here if needed
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
                    if (interactSoundOn != null)
                    {
                        interactSoundOn.Play();
                    }
                }
                else
                {
                    controlledLight.intensity = 0.05f;
                    if (interactSoundOff != null)
                    {
                        interactSoundOff.Play();
                    }
                }
                Debug.Log("Controlled Light Intensity: " + controlledLight.intensity);

                if (controlledLight.intensity == 1.0f && !sunlightAdded)
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
