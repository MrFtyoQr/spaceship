using UnityEngine;
using System;

public class BossSpawner : MonoBehaviour
{
    public GameConfig config;
    public BossHealthBar healthBar;

    Transform player;
    BossController currentBoss;
    int cycleMultiplier;

    public event Action OnBossDefeated;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SpawnBoss(int level)
    {
        BossData data = config.GetBossForLevel(level);
        if (data == null) return;

        cycleMultiplier = (level / 5 - 1) / config.bossOrder.Length;

        Camera cam = Camera.main;
        Vector3 spawnPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 10f));

        GameObject bossObj = new GameObject($"Boss_{data.bossName}");
        bossObj.transform.position = spawnPos;
        bossObj.layer = LayerMask.NameToLayer("Boss");

        var sr = bossObj.AddComponent<SpriteRenderer>();
        sr.sprite = data.sprite;
        sr.sortingOrder = 5;

        var rb = bossObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;

        var col = bossObj.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;

        var boss = bossObj.AddComponent<BossController>();
        boss.Setup(data, config.bossBulletPrefab, player);

        if (cycleMultiplier > 0)
            boss.ScaleStats(1f + cycleMultiplier * 0.5f);

        boss.OnBossDeath += HandleBossDeath;
        currentBoss = boss;

        if (healthBar != null)
            healthBar.Show(boss, data.bossName);
    }

    void HandleBossDeath()
    {
        if (currentBoss != null)
        {
            var scoreManager = FindAnyObjectByType<ScoreManager>();
            if (scoreManager != null)
                scoreManager.AddScore(currentBoss.PointsReward);

            if (config.explosionPrefab != null)
                Instantiate(config.explosionPrefab, currentBoss.transform.position, Quaternion.identity);

            currentBoss.OnBossDeath -= HandleBossDeath;
            currentBoss = null;
        }

        OnBossDefeated?.Invoke();
    }
}
