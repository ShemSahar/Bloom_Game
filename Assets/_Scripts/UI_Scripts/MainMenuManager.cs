using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip clickSound;  // Assign the sound to play when the button is clicked
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure there is an AudioSource component on this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure the GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);
    }

    public void StartTutorial()
    {
        PlaySoundAndLoadScene("Loading");
    }

    private void PlaySoundAndLoadScene(string sceneName)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
            DontDestroyOnLoad(audioSource.gameObject);  // Ensure the audio source is not destroyed
        }
        else
        {
            Debug.LogError("AudioSource or clickSound is not assigned.");
        }

        SceneManager.LoadScene(sceneName);
    }
}
