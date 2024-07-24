using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class ShadeButtonTutorial : MonoBehaviour, IInteractable
    {
        [Header("Shade Settings")]
        public float shadeSpeed = 1.0f; // Speed of shade rising
        public Transform[] shadeTransforms; // Assign the shade objects in the Inspector

        [Header("Interaction Settings")]
        public float interactRange = 2.0f; // Range within which the player can interact
        public Transform playerTransform; // Assign the player object in the Inspector
        public Button interactButton; // Assign the interaction button in the Inspector

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        [Header("Audio Settings")]
        public AudioSource interactSound;  // Reference to the AudioSource component for interaction sound

        private bool isIncreasing;
        private Coroutine risingCoroutine;
        private ResourceManager resourceManager;
        private bool hasInteracted = false;
        public MissionManager missionManager;  // Reference to the MissionManager

        private void Start()
        {
            resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager == null)
            {
                Debug.LogError("ResourceManager not found in the scene.");
            }

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (tutorialUI != null)
            {
                tutorialUI.SetActive(false);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the ShadeButton or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            if (interactSound == null)
            {
                Debug.LogError("Interact Sound is not assigned.");
            }
        }

        private void Update()
        {
            if (IsPlayerInRange())
            {
                ShowTutorialUI(true);
                outline.enabled = true;
            }
            else
            {
                ShowTutorialUI(false);
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
            if (!hasInteracted)
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

                if (interactSound != null)
                {
                    interactSound.Play();
                }

                AddLightResource();
                hasInteracted = true;
                missionManager.CompleteMission();  // Notify mission manager of completion
                outline.enabled = false;  // Toggle off the outline after interaction
            }
            StartRising();
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
                resourceManager.AddSunlight(50f);
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

        private void ShowTutorialUI(bool show)
        {
            if (tutorialUI != null)
            {
                tutorialUI.SetActive(show);
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
                interactButton.onClick.RemoveListener(OnInteractButtonClicked);
            }
        }
    }
}
