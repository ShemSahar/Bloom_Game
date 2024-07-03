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

        [Header("Light Settings")]
        public Light[] spotLights; // Assign the spotlight objects in the Inspector
        public float maxIntensity = 5.0f;
        public float lightSpeed = 1.0f; // Speed of intensity increase

        [Header("Interaction Settings")]
        public float interactRange = 2.0f; // Range within which the player can interact
        public Transform playerTransform; // Assign the player object in the Inspector
        public Button interactButton; // Assign the interaction button in the Inspector

        [Header("UI Settings")]
        public GameObject tutorialUI;  // Reference to the UI element (Text/Image) to show the tutorial instruction

        [Header("Outline Settings")]
        public Material outlineMaterial;  // Reference to the outline material
        public Color outlineColor = Color.blue;  // Color of the outline
        public float outlineWidth = 1.0f;  // Width of the outline

        private bool isIncreasing;
        private Coroutine risingCoroutine;
        private ResourceManager resourceManager;
        private Material originalMaterial;
        private bool hasInteracted = false;

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
            if (!hasInteracted)
            {
                AddLightResource();
                hasInteracted = true;
            }
            StartRising();
            StartIncreasing();
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
