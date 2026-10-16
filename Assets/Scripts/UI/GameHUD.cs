using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    public ScoreManager scoreManager;
    public LevelManager levelManager;
    PlayerHealth playerHealth;

    void Start()
    {
        if (scoreManager != null)
            scoreManager.OnScoreChanged += UpdateScore;
        if (levelManager != null)
            levelManager.OnLevelStart += UpdateLevel;

        UpdateScore(0);
    }

    void OnDestroy()
    {
        if (scoreManager != null)
            scoreManager.OnScoreChanged -= UpdateScore;
        if (levelManager != null)
            levelManager.OnLevelStart -= UpdateLevel;
        UnsubscribePlayer();
    }

    public void BindPlayer(PlayerHealth health)
    {
        UnsubscribePlayer();
        playerHealth = health;
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealth;
    }

    void UnsubscribePlayer()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealth;
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Puntos: {score}";
    }

    void UpdateLevel(int level, bool isBoss)
    {
        if (levelText != null)
            levelText.text = $"Nivel {level}";
    }

    void UpdateHealth(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
        if (healthText != null)
            healthText.text = $"HP: {current:0}/{max:0}";
    }
}
