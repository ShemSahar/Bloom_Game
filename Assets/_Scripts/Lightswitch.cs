using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        [Header("Light Settings")]
        public Light lightSource;
        public bool isOn = false;

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;
        public Transform playerTransform;
        public Button interactButton;

        private bool isInteractable = false;  // Indicates if the object is interactable

        private void Start()
        {
            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            TaskManager.Instance.taskObjects.Add(gameObject);
        }

        private void Update()
        {
            if (IsPlayerInRange() && isInteractable)
            {
                // Show UI or outline to indicate interactability
            }
            else
            {
                // Hide UI or outline
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
            isOn = !isOn;
            lightSource.enabled = isOn;
            TaskManager.Instance.CompleteTask();  // Mark task as complete
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

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
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
