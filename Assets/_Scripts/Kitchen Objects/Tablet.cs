using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MyGame
{
    public class Tablet : MonoBehaviour, IInteractable
    {
        [Header("Movement Settings")]
        public RectTransform uiObject;  // Assign the UI object (RectTransform) to move
        public float moveSpeed = 2f;  // Speed of the movement
        public AnimationCurve easeCurve;  // Ease-in and ease-out curve

        [Header("Interaction Settings")]
        public Transform playerTransform;  // Assign the player object in the Inspector
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Button interactButton;  // Assign the interaction button in the Inspector
        public Outline outline;  // Reference to the Outline component

        [Header("UI Elements")]
        public GameObject joystick;  // Reference to the on-screen joystick
        public Button[] onScreenButtons;  // Array of on-screen buttons to disable
        public Button closeButton;  // Reference to the close button in the Inspector
        public GameObject toggleGameObject;  // Reference to the game object to toggle

        private bool isInteracted = false;
        private bool isMovingOut = false;
        private bool isShown = false;

        private void Start()
        {
            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the object or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off
        }

        private void Update()
        {
            if (IsPlayerInRange())
            {
                outline.enabled = true;
            }
            else
            {
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
            if (!isInteracted)
            {
                StartCoroutine(MoveIn());
                isInteracted = true;
            }
        }

        private IEnumerator MoveIn()
        {
            Vector2 startPosition = new Vector2(uiObject.anchoredPosition.x, 1400);
            Vector2 finalPosition = new Vector2(uiObject.anchoredPosition.x, 0);

            // Move from start position to final position with ease-in and ease-out
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                float t = easeCurve.Evaluate(elapsedTime);
                uiObject.anchoredPosition = Vector2.Lerp(startPosition, finalPosition, t);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }
            uiObject.anchoredPosition = finalPosition;

            isShown = true;
            DisableInput();
        }

        private void DisableInput()
        {
            if (joystick != null)
            {
                joystick.SetActive(false);
            }

            foreach (Button button in onScreenButtons)
            {
                button.interactable = false;
            }

            EventSystem.current.SetSelectedGameObject(null); // Deselect any selected UI element
        }

        private void EnableInput()
        {
            if (joystick != null)
            {
                joystick.SetActive(true);
            }

            foreach (Button button in onScreenButtons)
            {
                button.interactable = true;
            }
        }

        private IEnumerator MoveOut()
        {
            isMovingOut = true;
            isShown = false;

            // Toggle the game object off before moving out
            if (toggleGameObject != null)
            {
                toggleGameObject.SetActive(false);
            }

            Vector2 startPosition = new Vector2(uiObject.anchoredPosition.x, 0);
            Vector2 endPosition = new Vector2(uiObject.anchoredPosition.x, 1400);

            // Move from start position to end position with ease-in and ease-out
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                float t = easeCurve.Evaluate(elapsedTime);
                uiObject.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }
            uiObject.anchoredPosition = endPosition;

            // Toggle the game object back on after moving out
            if (toggleGameObject != null)
            {
                toggleGameObject.SetActive(true);
            }

            isMovingOut = false;
            isInteracted = false;
            EnableInput();
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }

        private void OnDestroy()
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveListener(OnInteractButtonClicked);
            }
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            }
        }

        public void OnCloseButtonClicked()
        {
            if (!isMovingOut)
            {
                StartCoroutine(MoveOut());
            }
        }
    }
}
