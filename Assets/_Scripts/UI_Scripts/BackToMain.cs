using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Method to load the MainMenu scene, can be called from a button's OnClick event
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
