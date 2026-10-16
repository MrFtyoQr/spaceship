using UnityEngine;
using UnityEngine.InputSystem;

public class GameSetup : MonoBehaviour
{
    public GameConfig config;
    public InputActionAsset inputActions;
    public Transform spawnPoint;
    public GameHUD gameHUD;
    public BossHealthBar bossHealthBar;
    public LevelIntroUI levelIntroUI;
    public GameOverUI gameOverUI;
    public ScoreManager scoreManager;
    public LevelManager levelManager;
    public AsteroidSpawner asteroidSpawner;
    public BossSpawner bossSpawner;
    public ShipCamera shipCamera;

    void Start()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance == null)
        {
            var gmObj = new GameObject("GameManager");
            var gm = gmObj.AddComponent<GameManager>();
            gm.config = config;
        }
        else if (config == null)
        {
            config = GameManager.Instance.config;
        }

        if (GameManager.Instance.config == null)
            GameManager.Instance.config = config;

        asteroidSpawner.config = config;
        bossSpawner.config = config;
        bossSpawner.healthBar = bossHealthBar;

        SpawnPlayer();
        levelManager.StartGame();
    }

    void SpawnPlayer()
    {
        ShipData shipData = GameManager.Instance.SelectedShip;
        if (shipData == null || shipData.prefab == null)
        {
            Debug.LogError("No ship selected or ship prefab missing! Check that GameConfig has ships assigned.");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject playerObj = Instantiate(shipData.prefab, pos, Quaternion.identity);
        playerObj.name = "Player";
        playerObj.transform.localScale = Vector3.one * 3f;

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
            playerObj.layer = playerLayer;

        var rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = false;
            rb.linearDamping = 0.5f;
        }

        var compositeCol = playerObj.GetComponent<CompositeCollider2D>();
        if (compositeCol != null)
            compositeCol.isTrigger = true;

        var playerInput = playerObj.GetComponent<PlayerInput>();
        if (playerInput == null)
            playerInput = playerObj.AddComponent<PlayerInput>();

        if (inputActions != null)
            playerInput.actions = inputActions;
        else
        {
            var asset = Resources.Load<InputActionAsset>("InputSystem_Actions");
            if (asset != null) playerInput.actions = asset;
        }
        playerInput.defaultActionMap = "Player";
        playerInput.notificationBehavior = PlayerNotifications.SendMessages;

        var controller = playerObj.AddComponent<PlayerController>();
        controller.Setup(shipData);

        var health = playerObj.AddComponent<PlayerHealth>();
        health.Setup(shipData);

        var shooting = playerObj.AddComponent<PlayerShooting>();
        shooting.Setup(shipData, config.playerBulletPrefab);

        playerObj.AddComponent<ScreenWrapper>();

        health.OnPlayerDeath += HandlePlayerDeath;

        if (gameHUD != null)
            gameHUD.BindPlayer(health);

        if (shipCamera != null)
            shipCamera.target_object = playerObj.transform;

        if (bossSpawner != null)
            bossSpawner.SetPlayer(playerObj.transform);
    }

    void HandlePlayerDeath()
    {
        if (gameOverUI != null)
            gameOverUI.Show();

        asteroidSpawner.StopSpawning();
    }
}
