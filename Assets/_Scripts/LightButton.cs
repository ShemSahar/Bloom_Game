using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LightButton : MonoBehaviour, IInteractable
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

        [Header("Audio Settings")]
        public AudioClip interactSoundOn;  // Reference to the AudioSource component for the On sound
        public AudioClip interactSoundOff;  // Reference to the AudioSource component for the Off sound
        private AudioSource audioSource;  // Audio source to play the sounds

        private bool sunlightAdded = false;  // Ensure sunlight is only added once
        private bool lightStateChanged = false;

        private void Start()
        {
            if (controlledLight == null)
            {
                Debug.LogError("No Light component found on the LightButton or its children.");
            }
            else
            {
                controlledLight.enabled = false;  // Ensure the light is initially off
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
                    Debug.LogError("No Outline component found on the LightButton or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            // Add and configure the audio source
            audioSource = gameObject.AddComponent<AudioSource>();
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

                controlledLight.enabled = !controlledLight.enabled;

                if (controlledLight.enabled)
                {
                    if (!sunlightAdded)
                    {
                        AddSunlightToResourceManager();
                        sunlightAdded = true;
                    }
                    if (audioSource != null && interactSoundOn != null)
                    {
                        audioSource.PlayOneShot(interactSoundOn);
                    }
                }
                else
                {
                    if (audioSource != null && interactSoundOff != null)
                    {
                        audioSource.PlayOneShot(interactSoundOff);
                    }
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
