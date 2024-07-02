using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartTutorial()
    {
        SceneManager.LoadScene("Loading");
    }
}
