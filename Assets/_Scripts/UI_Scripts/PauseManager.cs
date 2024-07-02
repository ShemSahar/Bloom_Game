using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public Image screenTint;
    public TMP_Text pauseText;
    public Animator settingsAnimator;  // Animator component for the settings buttons animation

    private bool isPaused = false;

    private void Start()
    {
        if (screenTint != null)
        {
            screenTint.gameObject.SetActive(false);
        }

        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(false);
        }

        if (settingsAnimator != null)
        {
            settingsAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;  // Ensure the animator works in unscaled time
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }

        isPaused = !isPaused;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;  // Pause the game
        screenTint.gameObject.SetActive(true);  // Show the screen tint
        pauseText.gameObject.SetActive(true);  // Show the pause text

        if (settingsAnimator != null)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;  // Resume the game
        screenTint.gameObject.SetActive(false);  // Hide the screen tint
        pauseText.gameObject.SetActive(false);  // Hide the pause text

        if (settingsAnimator != null)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        settingsAnimator.SetTrigger("Show");
        yield return null;  // Wait for the next frame to ensure the animation starts
    }
}
