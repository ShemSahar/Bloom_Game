using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class BowlOfWater : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private float waterAmount = 10f; // The amount of water this bowl adds to the player's total
        [SerializeField]
        private float interactRange = 2.0f; // Range within which the player can interact
        [SerializeField]
        private Transform playerTransform; // Assign the player object in the Inspector
        [SerializeField]
        private Button interactButton; // Assign the button in the Inspector

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Child Settings")]
        public GameObject cylinder;  // Reference to the child Cylinder object

        private bool isFull = true; // Initial state of the bowl
        private bool waterAdded = false;
        public MissionManager missionManager;  // Reference to the MissionManager

        private void Start()
        {
            // Ensure the interactButton is assigned and set up the listener
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
                return;
            }
            interactButton.onClick.AddListener(HandleInteraction);

            if (tutorialUI != null)
            {
                tutorialUI.SetActive(false);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the BowlOfWater or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off

            if (cylinder == null)
            {
                Debug.LogError("Cylinder object is not assigned.");
            }
            else
            {
                ToggleCylinder(true); // Ensure the cylinder is visible at start
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

        private void HandleInteraction()
        {
            if (IsPlayerInRange())
            {
                Interact();
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

        public void Interact()
        {
            if (isFull && !waterAdded)
            {
                AddWaterToResourceManager();
                isFull = false;
                ToggleCylinder(false);
                waterAdded = true;  // Ensure water is added only once
                missionManager.CompleteMission();  // Notify mission manager of completion
                outline.enabled = false;  // Toggle off the outline after interaction
            }
        }

        private void AddWaterToResourceManager()
        {
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddWater(waterAmount);
                Debug.Log("Added Water: " + waterAmount);
            }
            else
            {
                Debug.LogError("ResourceManager not found in the scene.");
            }
        }

        private void ToggleCylinder(bool isVisible)
        {
            if (cylinder != null)
            {
                MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();
                if (cylinderRenderer != null)
                {
                    cylinderRenderer.enabled = isVisible;
                }
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
                interactButton.onClick.RemoveListener(HandleInteraction);
            }
        }
    }
}
