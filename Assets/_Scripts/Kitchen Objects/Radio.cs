using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class Radio : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private float interactRange = 2.0f; // Range within which the player can interact
        [SerializeField]
        private Transform playerTransform; // Assign the player object in the Inspector
        [SerializeField]
        private Button interactButton; // Assign the button in the Inspector

        [Header("Outline Settings")]
        public Outline outline;  // Reference to the Outline component

        [Header("Audio Settings")]
        public AudioSource[] audioTracks;  // Array of audio tracks
        private int currentTrackIndex = 0;

        [Header("Animator Settings")]
        public Animator playerAnimator;  // Reference to the player's animator
        public string interactAnimationTrigger = "Interact";  // Name of the trigger for the interact animation

        private bool isPlaying = false;
        private AudioSource currentAudio;

        private void Start()
        {
            // Ensure the interactButton is assigned and set up the listener
            if (interactButton == null)
            {
                Debug.LogError("Interact Button is not assigned.");
                return;
            }
            interactButton.onClick.AddListener(HandleInteraction);

            if (outline == null)
            {
                outline = GetComponent<Outline>();
                if (outline == null)
                {
                    Debug.LogError("No Outline component found on the Radio or its children.");
                }
            }
            outline.enabled = true;  // Start with the outline toggled on

            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator is not assigned.");
            }

            if (audioTracks == null || audioTracks.Length == 0)
            {
                Debug.LogError("No audio tracks assigned to the Radio.");
                return;
            }
            currentAudio = audioTracks[currentTrackIndex];
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

        private void HandleInteraction()
        {
            if (IsPlayerInRange())
            {
                Interact();
            }
        }

        public void Interact()
        {
            // Trigger the interact animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(interactAnimationTrigger);
                Debug.Log("Interact animation triggered.");
            }
            else
            {
                Debug.LogError("Player Animator is not assigned.");
                return;
            }

            if (isPlaying)
            {
                currentAudio.Stop();
                isPlaying = false;
            }
            else
            {
                currentTrackIndex = (currentTrackIndex + 1) % audioTracks.Length;
                currentAudio = audioTracks[currentTrackIndex];
                currentAudio.Play();
                isPlaying = true;
            }

            outline.enabled = false;  // Toggle off the outline after interaction
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
            // Draw a yellow sphere to visualize interact range in the editor
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