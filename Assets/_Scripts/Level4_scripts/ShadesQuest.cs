using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class ShadesQuest : MonoBehaviour, IInteractable
    {
        [Header("Shade Settings")]
        public float shadeSpeed = 1.0f; // Speed of shade rising
        public Transform[] shadeTransforms; // Assign the shade objects in the Inspector

        [Header("Light Settings")]
        public Light[] spotLights; // Assign the spotlight objects in the Inspector
        public float maxIntensity = 5.0f;
        public float lightSpeed = 1.0f; // Speed of intensity increase
        public float lightAmount = 50f;  // Amount of light to add to the player's resources

        [Header("Interaction Settings")]
        public float interactRange = 2.0f; // Range within which the player can interact
        public Transform playerTransform; // Assign the player object in the Inspector
        public Button interactButton; // Assign the interaction button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Audio Settings")]
        public AudioClip interactSound;  // Audio clip for the interaction sound
        private AudioSource audioSource;  // Audio source to play the sound

        [Header("Mission Manager")]
        public MissionManager missionManager;  // Reference to the MissionManager

        private bool isIncreasing;
        private Coroutine risingCoroutine;
        private ResourceManager resourceManager;

        private void Start()
        {
            foreach (Light light in spotLights)
            {
                if (light != null)
                {
                    light.intensity = 0.0f; // Initialize intensity
                }
            }

            resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager == null)
            {
                Debug.LogError("ResourceManager not found in the scene.");
            }

            // Ensure the interactButton is assigned and set up the listener
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
                return;
            }
            interactButton.onClick.AddListener(HandleInteraction);

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the ShadesQuest or its children.");
                }
            }
            outline.enabled = true;  // Start with the outline toggled on

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            audioSource = gameObject.AddComponent<AudioSource>();
            if (interactSound == null)
            {
                Debug.LogError("Interact sound is not assigned.");
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

        private void HandleInteraction()
        {
            if (IsPlayerInRange())
            {
                Interact();
            }
        }

        public void Interact()
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

            if (audioSource != null && interactSound != null)
            {
                audioSource.PlayOneShot(interactSound);
            }

            AddLightResource();
            StartRising();
            StartIncreasing();

            outline.enabled = false;  // Toggle off the outline after interaction

            // Inform the mission manager
            if (missionManager != null)
            {
                missionManager.CompleteMission();
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

        private void AddLightResource()
        {
            if (resourceManager != null)
            {
                resourceManager.AddSunlight(lightAmount);  // Add light amount to the player's resources
                Debug.Log("Added Light: " + lightAmount);
            }
        }

        public void StartRising()
        {
            if (risingCoroutine == null)
            {
                risingCoroutine = StartCoroutine(Rise());
            }
        }

        public void StopRising()
        {
            if (risingCoroutine != null)
            {
                StopCoroutine(risingCoroutine);
                risingCoroutine = null;
            }
        }

        private IEnumerator Rise()
        {
            while (true)
            {
                foreach (Transform shadeTransform in shadeTransforms)
                {
                    Vector3 scale = shadeTransform.localScale;
                    scale.y -= shadeSpeed * Time.deltaTime;
                    if (scale.y <= 0)
                    {
                        scale.y = 0;
                        shadeTransform.localScale = scale;
                        StopRising();
                        yield break;
                    }
                    shadeTransform.localScale = scale;
                }
                yield return null;
            }
        }

        public void StartIncreasing()
        {
            isIncreasing = true;
        }

        public void StopIncreasing()
        {
            isIncreasing = false;
        }

        private void IncreaseIntensity()
        {
            foreach (Light light in spotLights)
            {
                if (light.intensity < maxIntensity)
                {
                    light.intensity += lightSpeed * Time.deltaTime;
                    if (light.intensity > maxIntensity)
                    {
                        light.intensity = maxIntensity;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (isIncreasing)
            {
                IncreaseIntensity();
            }
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
                interactButton.onClick.RemoveListener(HandleInteraction);
            }
        }
    }
}
