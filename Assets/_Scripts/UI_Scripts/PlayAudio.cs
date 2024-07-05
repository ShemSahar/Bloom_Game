using UnityEngine;
using UnityEngine.UI;

public class PlaySoundButton : MonoBehaviour
{
    public AudioClip clickSound;  // Assign the sound to play when the button is clicked
    private AudioSource audioSource;

    private void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // Ensure the button has an OnClick listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
        else
        {
            Debug.LogError("No Button component found on this GameObject.");
        }

        // Ensure the AudioSource component is present
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogError("AudioSource or clickSound is not assigned.");
        }
    }
}
