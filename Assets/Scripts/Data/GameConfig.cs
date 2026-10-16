using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "GameConfig", menuName = "AsteroidCommand/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Ships")]
    public ShipData[] allShips;

    [Header("Bosses")]
    public BossData[] bossOrder;

    [Header("Asteroids")]
    public GameObject[] asteroidHuge;
    public GameObject[] asteroidBig;
    public GameObject[] asteroidMed;
    public GameObject[] asteroidSmall;

    [Header("Prefabs")]
    public GameObject playerBulletPrefab;
    public GameObject bossBulletPrefab;
    public GameObject explosionPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Difficulty Scaling")]
    public int baseAsteroidCount = 5;
    public int asteroidsPerLevel = 2;
    public float baseAsteroidSpeed = 1.5f;
    public float asteroidSpeedPerLevel = 0.2f;
    public float baseSpawnInterval = 2f;
    public float minSpawnInterval = 0.3f;
    public float spawnIntervalReduction = 0.1f;

    [Header("Points")]
    public int pointsSmallAsteroid = 100;
    public int pointsMedAsteroid = 75;
    public int pointsBigAsteroid = 50;
    public int pointsHugeAsteroid = 25;

    public int GetAsteroidCount(int level)
    {
        return baseAsteroidCount + level * asteroidsPerLevel;
    }

    public float GetAsteroidSpeed(int level)
    {
        return baseAsteroidSpeed + level * asteroidSpeedPerLevel;
    }

    public float GetSpawnInterval(int level)
    {
        return Mathf.Max(minSpawnInterval, baseSpawnInterval - level * spawnIntervalReduction);
    }

    public BossData GetBossForLevel(int level)
    {
        if (bossOrder == null || bossOrder.Length == 0) return null;
        int bossIndex = (level / 5 - 1) % bossOrder.Length;
        return bossOrder[bossIndex];
    }

    public GameObject GetRandomAsteroidPrefab(int level)
    {
        float r = Random.value;
        float hugeChance = Mathf.Clamp01(0.05f + level * 0.01f);
        float bigChance = Mathf.Clamp01(0.15f + level * 0.01f);
        float medChance = 0.35f;

        if (r < hugeChance && asteroidHuge.Length > 0)
            return asteroidHuge[Random.Range(0, asteroidHuge.Length)];
        if (r < hugeChance + bigChance && asteroidBig.Length > 0)
            return asteroidBig[Random.Range(0, asteroidBig.Length)];
        if (r < hugeChance + bigChance + medChance && asteroidMed.Length > 0)
            return asteroidMed[Random.Range(0, asteroidMed.Length)];
        if (asteroidSmall.Length > 0)
            return asteroidSmall[Random.Range(0, asteroidSmall.Length)];

        return asteroidMed.Length > 0 ? asteroidMed[0] : null;
    }
}
