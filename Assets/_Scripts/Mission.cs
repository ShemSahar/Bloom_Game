using UnityEngine;

namespace MyGame
{
    [System.Serializable]
    public class Mission
    {
        public string missionName;
        public GameObject interactableObject;
        private MonoBehaviour interactableScript;
        private Outline interactableOutline;

        public void SetInteractable(bool state)
        {
            if (interactableObject != null)
            {
                interactableScript = interactableObject.GetComponent<MonoBehaviour>();
                interactableOutline = interactableObject.GetComponent<Outline>();

                if (interactableScript != null)
                {
                    interactableScript.enabled = state;
                }
                else
                {
                    Debug.LogError("Interactable script not found on " + interactableObject.name);
                }

                if (interactableOutline != null)
                {
                    interactableOutline.enabled = false;  // Ensure outline is always off initially
                }
                else
                {
                    Debug.LogError("Outline component not found on " + interactableObject.name);
                }
            }
            else
            {
                Debug.LogError("Interactable object is not assigned for mission: " + missionName);
            }
        }

        public void CompleteMission()
        {
            // Logic to mark mission as complete (e.g., updating UI, saving progress, etc.)
            // Set the outline to off and disable the interactable script after mission is complete
            if (interactableScript != null)
            {
                interactableScript.enabled = false;
            }

            if (interactableOutline != null)
            {
                interactableOutline.enabled = false;
            }
        }
    }
}
