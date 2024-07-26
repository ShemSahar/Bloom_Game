using UnityEngine;
using UnityEngine.UI;

public class UIButtonHandler : MonoBehaviour
{
    public GameManager gameManager;

    public void OnPlayButton()
    {
        gameManager.ShowGameUI();
    }

    public void OnExitButton()
    {
        gameManager.ExitGame();
    }
}
