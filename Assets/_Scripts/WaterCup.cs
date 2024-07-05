using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class WaterCup : MonoBehaviour, IInteractable
    {
        [Header("Water Cup Settings")]
        public float waterAmount = 50f;  // Amount of water to add to the player's resources

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

        private bool isInteractable = false;  // Indicates if the object is interactable
        private Material originalMaterial;

        public MissionManager missionManager;  // Reference to the MissionManager

        private void Start()
        {
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

            TaskManager.Instance.taskObjects.Add(gameObject);
        }

        private void Update()
        {
            if (IsPlayerInRange() && isInteractable)
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
            if (IsPlayerInRange() && isInteractable)
            {
                Interact();
            }
        }

        public void Interact()
        {
            // Logic to add water to player's resources
            missionManager?.CompleteMission();  // Mark mission as completed
            TaskManager.Instance.CompleteTask();
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

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(OnInteractButtonClicked);
            }
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }
    }
}
