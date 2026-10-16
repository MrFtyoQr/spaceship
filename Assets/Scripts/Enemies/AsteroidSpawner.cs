using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
    public GameConfig config;
    int totalToSpawn;
    int spawned;
    int activeCount;
    float spawnInterval;
    float asteroidSpeed;
    int currentLevel;
    bool spawning;
    Camera cam;

    public event System.Action OnAllAsteroidsCleared;

    void Start()
    {
        cam = Camera.main;
        AsteroidController.OnAsteroidDestroyed += HandleAsteroidDestroyed;
    }

    void OnDestroy()
    {
        AsteroidController.OnAsteroidDestroyed -= HandleAsteroidDestroyed;
    }

    public void StartSpawning(int level)
    {
        currentLevel = level;
        totalToSpawn = config.GetAsteroidCount(level);
        spawnInterval = config.GetSpawnInterval(level);
        asteroidSpeed = config.GetAsteroidSpeed(level);
        spawned = 0;
        activeCount = 0;
        spawning = true;
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        spawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnRoutine()
    {
        while (spawned < totalToSpawn && spawning)
        {
            SpawnAsteroid();
            spawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnAsteroid()
    {
        GameObject prefab = config.GetRandomAsteroidPrefab(currentLevel);
        if (prefab == null) return;

        Vector3 spawnPos = GetSpawnPosition();
        GameObject asteroid = Instantiate(prefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

        var controller = asteroid.GetComponent<AsteroidController>();
        if (controller == null)
            controller = asteroid.AddComponent<AsteroidController>();

        var rb = asteroid.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = asteroid.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        var collider = asteroid.GetComponent<Collider2D>();
        if (collider == null)
        {
            var circle = asteroid.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
        }
        else
        {
            collider.isTrigger = true;
        }

        var wrapper = asteroid.GetComponent<ScreenWrapper>();
        if (wrapper == null)
            asteroid.AddComponent<ScreenWrapper>();

        Vector2 direction = ((Vector2)cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)) - (Vector2)spawnPos).normalized;
        direction = Quaternion.Euler(0, 0, Random.Range(-30f, 30f)) * direction;
        controller.SetVelocity(direction * Random.Range(asteroidSpeed * 0.7f, asteroidSpeed * 1.3f));

        activeCount++;
    }

    Vector3 GetSpawnPosition()
    {
        int edge = Random.Range(0, 4);
        float offset = Random.Range(0f, 1f);
        Vector3 viewportPos;

        switch (edge)
        {
            case 0: viewportPos = new Vector3(offset, 1.15f, 10f); break;
            case 1: viewportPos = new Vector3(offset, -0.15f, 10f); break;
            case 2: viewportPos = new Vector3(-0.15f, offset, 10f); break;
            default: viewportPos = new Vector3(1.15f, offset, 10f); break;
        }

        return cam.ViewportToWorldPoint(viewportPos);
    }

    void HandleAsteroidDestroyed(int points)
    {
        activeCount--;
        if (activeCount <= 0 && spawned >= totalToSpawn)
            OnAllAsteroidsCleared?.Invoke();
    }
}
