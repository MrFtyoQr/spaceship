using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI pointsEarnedText;
    public Button restartButton;
    public Button mainMenuButton;
    public ScoreManager scoreManager;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(Restart);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void Show()
    {
        Time.timeScale = 0f;
        if (panel != null) panel.SetActive(true);

        int score = scoreManager != null ? scoreManager.SessionScore : 0;
        if (finalScoreText != null)
            finalScoreText.text = $"Puntos: {score}";
        if (pointsEarnedText != null)
            pointsEarnedText.text = $"+{score} puntos ganados!";

        if (scoreManager != null)
            scoreManager.FinalizeScore();
    }

    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
