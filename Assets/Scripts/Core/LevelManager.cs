using UnityEngine;
using System;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public AsteroidSpawner asteroidSpawner;
    public BossSpawner bossSpawner;

    int currentLevel;

    public event Action<int, bool> OnLevelStart;
    public event Action<int> OnLevelComplete;

    public int CurrentLevel => currentLevel;

    public void StartGame()
    {
        currentLevel = 0;
        NextLevel();
    }

    void NextLevel()
    {
        currentLevel++;
        bool isBoss = currentLevel % 5 == 0;
        OnLevelStart?.Invoke(currentLevel, isBoss);
        StartCoroutine(LevelIntroDelay(isBoss));
    }

    IEnumerator LevelIntroDelay(bool isBoss)
    {
        yield return new WaitForSeconds(2.5f);

        if (isBoss)
            StartBossLevel();
        else
            StartAsteroidLevel();
    }

    void StartAsteroidLevel()
    {
        asteroidSpawner.OnAllAsteroidsCleared += HandleLevelCleared;
        asteroidSpawner.StartSpawning(currentLevel);
    }

    void StartBossLevel()
    {
        if (bossSpawner != null)
        {
            bossSpawner.OnBossDefeated += HandleLevelCleared;
            bossSpawner.SpawnBoss(currentLevel);
        }
    }

    void HandleLevelCleared()
    {
        asteroidSpawner.OnAllAsteroidsCleared -= HandleLevelCleared;
        if (bossSpawner != null)
            bossSpawner.OnBossDefeated -= HandleLevelCleared;

        OnLevelComplete?.Invoke(currentLevel);
        StartCoroutine(LevelCompleteDelay());
    }

    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(2f);
        NextLevel();
    }
}
