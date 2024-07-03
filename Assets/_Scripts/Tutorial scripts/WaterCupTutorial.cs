using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class WaterCupTutorial : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private float waterAmount = 10f; // The amount of water this cup adds to the player's total
        [SerializeField]
        private Material fullMaterial;   // Material when the cup is full
        [SerializeField]
        private Material emptyMaterial;  // Material when the cup is empty
        [SerializeField]
        private float interactRange = 2.0f; // Range within which the player can interact
        [SerializeField]
        private Transform playerTransform; // Assign the player object in the Inspector
        [SerializeField]
        private Button interactButton; // Assign the button in the Inspector

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Material outlineMaterial;  // Reference to the outline material
        public Color outlineColor = Color.blue;  // Color of the outline
        public float outlineWidth = 1.0f;  // Width of the outline

        private bool isFull = true; // Initial state of the cup
        private MeshRenderer meshRenderer; // To change the material of the cup
        private Material originalMaterial;
        private bool waterAdded = false;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            SetMaterial(isFull ? fullMaterial : emptyMaterial);

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

            if (outlineMaterial != null)
            {
                outlineMaterial.SetColor("_BaseColor", outlineColor);
                outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
            }

            if (meshRenderer != null)
            {
                originalMaterial = meshRenderer.material;
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
                SetMaterial(emptyMaterial);
                waterAdded = true;  // Ensure water is added only once
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

        private void SetMaterial(Material material)
        {
            if (meshRenderer && material)
            {
                meshRenderer.material = material;
            }
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
            if (meshRenderer != null)
            {
                meshRenderer.material = enable ? outlineMaterial : originalMaterial;
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
