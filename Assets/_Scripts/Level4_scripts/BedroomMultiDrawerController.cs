using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class BedroomMultiDrawerController : MonoBehaviour, IInteractable
    {
        [Header("Drawer Settings")]
        public Transform[] drawerTransforms;  // Assign the target drawer objects in the Inspector
        public float[] moveDistances;  // Distances to move each drawer along the X axis
        public float[] moveSpeeds;  // Speeds at which each drawer moves

        [Header("Interaction Settings")]
        public float interactRange = 2.0f;  // Range within which the player can interact
        public Transform playerTransform;  // Assign the player object in the Inspector
        public Button interactButton;  // Assign the interaction button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interaction animation

        private Vector3[] initialPositions;
        private bool[] isOpen;
        private bool hasInteracted = false;

        private void Start()
        {
            if (drawerTransforms.Length != moveDistances.Length || drawerTransforms.Length != moveSpeeds.Length)
            {
                Debug.LogError("DrawerTransforms, MoveDistances, and MoveSpeeds arrays must have the same length.");
                return;
            }

            initialPositions = new Vector3[drawerTransforms.Length];
            isOpen = new bool[drawerTransforms.Length];

            for (int i = 0; i < drawerTransforms.Length; i++)
            {
                if (drawerTransforms[i] == null)
                {
                    Debug.LogError($"DrawerTransform {i} is not assigned.");
                    return;
                }

                initialPositions[i] = drawerTransforms[i].position;
                isOpen[i] = false;
            }

            if (interactButton != null)
            {
                interactButton.onClick.AddListener(OnInteractButtonClicked);
            }

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the BedroomMultiDrawerController or its children.");
                }
            }
            outline.enabled = false;  // Start with the outline toggled off

            // Check if playerAnimator is assigned
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned in the BedroomMultiDrawerController script.");
            }
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
            if (!hasInteracted)
            {
                // Trigger the interact animation
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(interactAnimationTrigger);
                    Debug.Log("Interact animation triggered.");
                }

                for (int i = 0; i < drawerTransforms.Length; i++)
                {
                    StartCoroutine(MoveDrawer(i));
                }

                outline.enabled = false;  // Toggle off the outline after interaction
                hasInteracted = true;
            }
            else
            {
                for (int i = 0; i < drawerTransforms.Length; i++)
                {
                    StartCoroutine(MoveDrawer(i));
                }
            }
        }

        private IEnumerator MoveDrawer(int index)
        {
            Vector3 targetPosition = isOpen[index] ? initialPositions[index] : initialPositions[index] + new Vector3(moveDistances[index], 0, 0);
            Vector3 startPosition = drawerTransforms[index].position;

            float elapsedTime = 0f;
            while (elapsedTime < moveSpeeds[index])
            {
                drawerTransforms[index].position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveSpeeds[index]);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            drawerTransforms[index].position = targetPosition;
            isOpen[index] = !isOpen[index];
        }

        public bool IsDrawerOpen(int index)
        {
            return isOpen[index];
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
        }
    }
}
