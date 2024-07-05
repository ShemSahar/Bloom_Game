using UnityEngine;
using UnityEngine.UI; // Make sure to include this namespace

namespace MyGame
{
    public class WaterCup : MonoBehaviour, IInteractable
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

        private bool isFull = true; // Initial state of the cup
        private MeshRenderer meshRenderer; // To change the material of the cup

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
        }

        private void HandleInteraction()
        {
            if (IsPlayerInRange() && isFull)
            {
                Interact();
            }
        }

        private bool IsPlayerInRange()
        {
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                return distanceToPlayer <= interactRange && IsGizmosColliding();
            }
            return false;
        }

        private bool IsGizmosColliding()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.transform == playerTransform)
                {
                    return true;
                }
            }
            return false;
        }

        public void Interact()
        {
            if (isFull)
            {
                AddWaterToResourceManager();
                isFull = false;
                SetMaterial(emptyMaterial);
            }
        }

        private void AddWaterToResourceManager()
        {
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddWater(waterAmount);
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

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position to visualize interact range in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
