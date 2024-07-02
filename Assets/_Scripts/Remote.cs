using UnityEngine;

namespace MyGame
{
    public class Remote : MonoBehaviour, IInteractable
    {
        public Light controlledLight;  // Reference to the Light component
        private bool hasInteractedAutomatically = false; // Flag to track automatic interaction

        [SerializeField]
        private LayerMask interactLayer;  // Layer that triggers automatic interaction

        void Start()
        {
            // Check for the Light component
            if (controlledLight == null)
            {
                controlledLight = GetComponentInChildren<Light>();
                if (controlledLight == null)
                {
                    Debug.LogError("No Light component found on the Remote or its children.");
                }
            }

            // Ensure a BoxCollider is added and set as non-trigger for physical blocking
            BoxCollider collider = gameObject.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider>();
                collider.isTrigger = false; // This will now physically block the object
            }
            else
            {
                collider.isTrigger = false; // Ensure it is not a trigger
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            // Check if the collision is with the specified interaction layer and control hasn't been taken over manually
            if (((1 << collision.gameObject.layer) & interactLayer) != 0 && !hasInteractedAutomatically)
            {
                // Turn on the light if it's the first interaction and hasn't been turned on manually
                if (controlledLight != null && !controlledLight.enabled)
                {
                    controlledLight.enabled = true;
                    hasInteractedAutomatically = true; // Ensure this happens only once
                }
            }
        }

        public void Interact()
        {
            // Toggle the light on and off manually after the first automatic interaction
            if (controlledLight != null)
            {
                controlledLight.enabled = !controlledLight.enabled;
            }
            else
            {
                Debug.LogError("Controlled light not assigned or found.");
            }
        }
    }
}
