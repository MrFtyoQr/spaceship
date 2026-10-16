using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    int sessionScore;

    public event Action<int> OnScoreChanged;

    public int SessionScore => sessionScore;

    void Start()
    {
        sessionScore = 0;
        AsteroidController.OnAsteroidDestroyed += HandleAsteroidPoints;
    }

    void OnDestroy()
    {
        AsteroidController.OnAsteroidDestroyed -= HandleAsteroidPoints;
    }

    void HandleAsteroidPoints(int points)
    {
        if (points > 0) AddScore(points);
    }

    public void AddScore(int amount)
    {
        sessionScore += amount;
        OnScoreChanged?.Invoke(sessionScore);
    }

    public void FinalizeScore()
    {
        GameManager.Instance.AddPoints(sessionScore);
        SaveManager.SaveHighScore(sessionScore);
    }
}
