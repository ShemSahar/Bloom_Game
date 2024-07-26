using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameUI;
    public GameObject exitButton;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    private int player1Score = 0;
    private int player2Score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        gameUI.SetActive(false);
        exitButton.SetActive(false);
        UpdateScore();
    }

    public void ShowGameUI()
    {
        gameUI.SetActive(true);
        exitButton.SetActive(true);
    }

    public void ExitGame()
    {
        gameUI.SetActive(false);
        exitButton.SetActive(false);
    }

    public void AddScore(int player)
    {
        if (player == 1)
        {
            player1Score++;
        }
        else if (player == 2)
        {
            player2Score++;
        }
        UpdateScore();
    }

    private void UpdateScore()
    {
        player1ScoreText.text = "Player 1: " + player1Score;
        player2ScoreText.text = "Player 2: " + player2Score;
    }

    public void StartGame()
    {
        // Called when the game restarts
    }
}
