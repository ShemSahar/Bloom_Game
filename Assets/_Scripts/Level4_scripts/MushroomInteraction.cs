using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MushroomInteraction : MonoBehaviour, IInteractable
{
    public VideoPlayer videoPlayer;  // Reference to the VideoPlayer component
    public Transform playerTransform;  // Reference to the player Transform
    public float interactRange = 2.0f;  // Range within which the player can interact
    public Button interactButton;  // Reference to the interact Button

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(HandleInteraction);
        }
        else
        {
            Debug.LogError("Interact Button is not assigned.");
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("VideoPlayer is not assigned.");
        }
    }

    private void Update()
    {
        if (IsPlayerInRange())
        {
            // Optional: Show some UI or indication that the player can interact
        }
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

    private void HandleInteraction()
    {
        if (IsPlayerInRange())
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    private void OnDestroy()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(HandleInteraction);
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("Loading4");
    }
}
