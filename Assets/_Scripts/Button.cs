using System.Collections;
using UnityEngine;

namespace MyGame
{
    public class ShadeLightButton : MonoBehaviour, IInteractable
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
        }

        private void Update()
        {
            // Handle continuous pressing of the interact key
            HandleInteraction();
        }

        public void Interact()
        {
            if (IsPlayerInRange())
            {
                AddLightResource();
                StartRising();
                StartIncreasing();
            }
        }

        private void HandleInteraction()
        {
            // The interact key handling will be done by the external script
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

        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position to visualize interact range in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }

        public void SetInteractable(bool isActive)
        {
            throw new System.NotImplementedException();
        }
    }
}
