using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    // Method to restart the current scene, can be called from a button's OnClick event
    public void RestartScene()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current active scene
        SceneManager.LoadScene(currentScene.name);
    }
}
