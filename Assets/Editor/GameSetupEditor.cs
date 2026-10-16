using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GameSetupEditor : EditorWindow
{
    [MenuItem("Asteroid Command/Setup Entire Game")]
    public static void SetupGame()
    {
        CreateFolders();
        CreateScriptableObjects();
        CreatePrefabs();
        CreateMainMenuScene();
        CreateShipSelectScene();
        CreateGameScene();
        SetupBuildSettings();
        SetupPhysicsLayers();
        Debug.Log("Asteroid Command setup complete!");
    }

    static void CreateFolders()
    {
        string[] folders = {
            "Assets/Prefabs",
            "Assets/ScriptableObjects/Ships",
            "Assets/ScriptableObjects/Bosses"
        };
        foreach (var f in folders)
        {
            if (!AssetDatabase.IsValidFolder(f))
            {
                string parent = Path.GetDirectoryName(f).Replace("\\", "/");
                string name = Path.GetFileName(f);
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }

    static void CreateScriptableObjects()
    {
        // Ship data
        string[] tinyShipNames = {
            "Scout", "Raider", "Hawk", "Falcon", "Viper",
            "Phantom", "Striker", "Blaze", "Comet", "Nova",
            "Thunder", "Raptor", "Storm", "Eclipse", "Shadow",
            "Titan", "Phoenix", "Omega", "Valkyrie", "Monarch"
        };
        int[] costs = { 0, 500, 1000, 2000, 3500, 5000, 7000, 10000, 13000, 17000,
                        21000, 26000, 32000, 39000, 47000, 56000, 66000, 78000, 92000, 110000 };
        float[] speeds = { 5f, 5.5f, 4.5f, 6f, 7f, 5f, 6.5f, 4f, 8f, 5.5f,
                           6f, 7.5f, 4.5f, 6f, 8.5f, 3.5f, 7f, 5f, 9f, 6.5f };
        float[] fireRates = { 3f, 3.5f, 4f, 3f, 2.5f, 5f, 3.5f, 6f, 2f, 4f,
                              3.5f, 2.5f, 5.5f, 4f, 3f, 4.5f, 3.5f, 6.5f, 2.5f, 5f };
        float[] damages = { 1f, 1.2f, 1f, 1.5f, 1f, 0.8f, 2f, 0.7f, 1.5f, 1.3f,
                            2f, 1.8f, 0.9f, 2.5f, 1.2f, 3f, 2.2f, 1f, 2f, 3.5f };
        float[] healths = { 100f, 90f, 120f, 80f, 70f, 130f, 85f, 150f, 60f, 110f,
                            100f, 75f, 160f, 90f, 65f, 200f, 95f, 180f, 70f, 120f };

        var gameConfig = ScriptableObject.CreateInstance<GameConfig>();

        gameConfig.allShips = new ShipData[20];
        for (int i = 0; i < 20; i++)
        {
            var ship = ScriptableObject.CreateInstance<ShipData>();
            ship.shipName = tinyShipNames[i];
            ship.shipIndex = i;
            ship.moveSpeed = speeds[i];
            ship.rotationSpeed = 300f;
            ship.fireRate = fireRates[i];
            ship.bulletDamage = damages[i];
            ship.bulletSpeed = 10f + i * 0.3f;
            ship.maxHealth = healths[i];
            ship.unlockCost = costs[i];

            string prefabPath = $"Assets/Tiny Ships/Prefabs/TinyShip{i + 1}.prefab";
            ship.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            string spritePath = $"Assets/Tiny Ships/Art/tinyShip{i + 1}.png";
            var sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);
            foreach (var s in sprites)
            {
                if (s is Sprite sprite && sprite.name.Contains("0"))
                {
                    ship.icon = sprite;
                    break;
                }
            }
            if (ship.icon == null)
            {
                foreach (var s in sprites)
                {
                    if (s is Sprite sprite)
                    {
                        ship.icon = sprite;
                        break;
                    }
                }
            }

            ship.description = $"A {(speeds[i] > 6f ? "fast" : speeds[i] < 5f ? "slow but tough" : "balanced")} ship with {(fireRates[i] > 4f ? "rapid fire" : "powerful shots")}.";

            string shipPath = $"Assets/ScriptableObjects/Ships/Ship{(i + 1):D2}_{tinyShipNames[i]}.asset";
            AssetDatabase.CreateAsset(ship, shipPath);
            gameConfig.allShips[i] = ship;
        }

        // Boss data
        string[] bossNames = { "Fighter 1", "Fighter 2", "Alien 1", "Frigate 1", "Alien 2",
                               "Destroyer 1", "Cruiser 1", "Alien 3", "Carrier 1", "Huge 1", "Station 2" };
        string[] bossSpriteFiles = { "Fighter 1.png", "Fighter 2.png", "Alien 1.png", "Frigate 1.png", "Alien 2.png",
                                     "Destroyer 1.png", "Cruiser 1.png", "Alien 3.png", "Carrier 1.png", "Huge 1.png", "Station 2.png" };
        float[] bossHPs = { 150f, 200f, 250f, 350f, 300f, 500f, 600f, 450f, 800f, 1200f, 1500f };
        float[] bossSpeeds = { 3f, 4f, 3.5f, 2f, 5f, 1.5f, 2f, 5f, 1f, 0.8f, 0f };
        float[] bossFireRates = { 1f, 1.5f, 1.2f, 0.8f, 1.5f, 0.5f, 0.7f, 2f, 0.6f, 0.4f, 1f };
        float[] bossDamages = { 10f, 12f, 15f, 20f, 12f, 30f, 25f, 15f, 20f, 35f, 25f };
        int[] bossRewards = { 500, 750, 1000, 1500, 1200, 2000, 2500, 2000, 3500, 5000, 7500 };
        bool[] bossShields = { false, false, false, true, false, false, true, true, false, true, true };
        int[] bossBulletCounts = { 1, 1, 3, 1, 2, 1, 3, 3, 5, 5, 4 };
        float[] bossBulletSpreads = { 0f, 0f, 30f, 0f, 15f, 0f, 45f, 40f, 60f, 90f, 360f };
        BossMovementPattern[] bossPatterns = {
            BossMovementPattern.Strafe, BossMovementPattern.Strafe,
            BossMovementPattern.Orbit, BossMovementPattern.Strafe,
            BossMovementPattern.ChargeAndRetreat, BossMovementPattern.ChargeAndRetreat,
            BossMovementPattern.Orbit, BossMovementPattern.ChargeAndRetreat,
            BossMovementPattern.Strafe, BossMovementPattern.Orbit,
            BossMovementPattern.Stationary
        };

        gameConfig.bossOrder = new BossData[11];
        for (int i = 0; i < 11; i++)
        {
            var boss = ScriptableObject.CreateInstance<BossData>();
            boss.bossName = bossNames[i];
            boss.maxHealth = bossHPs[i];
            boss.moveSpeed = bossSpeeds[i];
            boss.fireRate = bossFireRates[i];
            boss.bulletDamage = bossDamages[i];
            boss.bulletSpeed = 5f + i * 0.3f;
            boss.pointsReward = bossRewards[i];
            boss.hasShield = bossShields[i];
            boss.shieldDuration = 3f;
            boss.shieldCooldown = 8f;
            boss.bulletCount = bossBulletCounts[i];
            boss.bulletSpreadAngle = bossBulletSpreads[i];
            boss.bulletColor = Color.HSVToRGB((float)i / 11f, 0.8f, 1f);
            boss.movementPattern = bossPatterns[i];
            boss.orbitRadius = 3f + i * 0.3f;
            boss.chargeSpeed = 6f + i * 0.5f;

            string spritePath = $"Assets/2D Space Kit/Ships and Stations/{bossSpriteFiles[i]}";
            boss.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            string bossPath = $"Assets/ScriptableObjects/Bosses/Boss{(i + 1):D2}_{bossNames[i].Replace(" ", "")}.asset";
            AssetDatabase.CreateAsset(boss, bossPath);
            gameConfig.bossOrder[i] = boss;
        }

        // Asteroid references
        string asteroidPrefabPath = "Assets/2D pixel asteroids/Prefabs/";
        gameConfig.asteroidHuge = new GameObject[] {
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterHuge1.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterHuge2.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterHuge3.prefab")
        };
        gameConfig.asteroidBig = new GameObject[] {
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterBig1.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterBig2.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterBig3.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterBig4.prefab")
        };
        gameConfig.asteroidMed = new GameObject[] {
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterMed1.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterMed2.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterMed3.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterMed4.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterMed5.prefab")
        };
        gameConfig.asteroidSmall = new GameObject[] {
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterSmall1.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterSmall2.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterSmall3.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterSmall4.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterSmall5.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(asteroidPrefabPath + "AsterSmall6.prefab")
        };

        // Explosion prefab
        gameConfig.explosionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/2D Space Kit/Prefabs/Explosion.prefab");
        gameConfig.muzzleFlashPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/2D Space Kit/Prefabs/Muzzle Flash.prefab");

        AssetDatabase.CreateAsset(gameConfig, "Assets/ScriptableObjects/GameConfig.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("ScriptableObjects created successfully!");
    }

    static void CreatePrefabs()
    {
        // PlayerBullet prefab
        GameObject bulletObj = new GameObject("PlayerBullet");
        var sr = bulletObj.AddComponent<SpriteRenderer>();
        sr.color = Color.cyan;
        sr.sortingOrder = 10;
        var bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/2D Space Kit/Missiles/Missile (1).png");
        if (bulletSprite != null) sr.sprite = bulletSprite;

        var bc = bulletObj.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.1f, 0.3f);

        var rb = bulletObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        bulletObj.AddComponent<Bullet>();

        var trail = bulletObj.AddComponent<TrailRenderer>();
        trail.time = 0.15f;
        trail.startWidth = 0.05f;
        trail.endWidth = 0f;
        trail.startColor = Color.cyan;
        trail.endColor = new Color(0, 1, 1, 0);
        trail.material = new Material(Shader.Find("Sprites/Default"));

        PrefabUtility.SaveAsPrefabAsset(bulletObj, "Assets/Prefabs/PlayerBullet.prefab");
        DestroyImmediate(bulletObj);

        // BossBullet prefab
        GameObject bossBulletObj = new GameObject("BossBullet");
        var bsr = bossBulletObj.AddComponent<SpriteRenderer>();
        bsr.color = Color.red;
        bsr.sortingOrder = 10;
        var missileSpr = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/2D Space Kit/Missiles/Missile (3).png");
        if (missileSpr != null) bsr.sprite = missileSpr;

        var bbc = bossBulletObj.AddComponent<BoxCollider2D>();
        bbc.isTrigger = true;
        bbc.size = new Vector2(0.15f, 0.4f);

        var brb = bossBulletObj.AddComponent<Rigidbody2D>();
        brb.gravityScale = 0f;
        brb.bodyType = RigidbodyType2D.Dynamic;
        brb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        bossBulletObj.AddComponent<BossBullet>();

        PrefabUtility.SaveAsPrefabAsset(bossBulletObj, "Assets/Prefabs/BossBullet.prefab");
        DestroyImmediate(bossBulletObj);

        // Wire bullet prefabs into GameConfig
        var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
        if (config != null)
        {
            config.playerBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayerBullet.prefab");
            config.bossBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BossBullet.prefab");
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        Debug.Log("Prefabs created successfully!");
    }

    static void CreateMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Background
        var bgObj = new GameObject("Background");
        var bgSr = bgObj.AddComponent<SpriteRenderer>();
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/2D Space Kit/Backgrounds/1.png");
        if (bgSprite != null) bgSr.sprite = bgSprite;
        bgSr.sortingOrder = -10;
        bgObj.transform.localScale = Vector3.one * 2f;

        // GameManager
        var gmObj = new GameObject("GameManager");
        var gm = gmObj.AddComponent<GameManager>();
        var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
        gm.config = config;

        // Canvas
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Title
        var titleObj = CreateTMPText(canvasObj.transform, "TitleText", "ASTEROID COMMAND\nComandante Espacial", 72,
            new Vector2(0, 200), new Vector2(800, 100));
        titleObj.GetComponent<TextMeshProUGUI>().color = Color.cyan;

        // Play Button
        CreateButton(canvasObj.transform, "PlayButton", "JUGAR", new Vector2(0, 0), new Vector2(300, 60));

        // Ship Select Button
        CreateButton(canvasObj.transform, "ShipSelectButton", "ELEGIR NAVE", new Vector2(0, -80), new Vector2(300, 60));

        // Quit Button
        CreateButton(canvasObj.transform, "QuitButton", "SALIR", new Vector2(0, -160), new Vector2(300, 60));

        // Points display
        CreateTMPText(canvasObj.transform, "PointsText", "Puntos: 0", 28,
            new Vector2(-30, -30), new Vector2(300, 50), TextAlignmentOptions.TopRight,
            new Vector2(1, 1), new Vector2(1, 1));

        // MainMenuUI component
        var menuUI = canvasObj.AddComponent<MainMenuUI>();
        menuUI.playButton = canvasObj.transform.Find("PlayButton").GetComponent<Button>();
        menuUI.shipSelectButton = canvasObj.transform.Find("ShipSelectButton").GetComponent<Button>();
        menuUI.quitButton = canvasObj.transform.Find("QuitButton").GetComponent<Button>();
        menuUI.pointsText = canvasObj.transform.Find("PointsText").GetComponent<TextMeshProUGUI>();

        // EventSystem
        var eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("MainMenu scene created!");
    }

    static void CreateShipSelectScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Background
        var bgObj = new GameObject("Background");
        var bgSr = bgObj.AddComponent<SpriteRenderer>();
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/2D Space Kit/Backgrounds/1.png");
        if (bgSprite != null) bgSr.sprite = bgSprite;
        bgSr.sortingOrder = -10;
        bgObj.transform.localScale = Vector3.one * 2f;

        // Canvas
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Header
        CreateTMPText(canvasObj.transform, "TitleText", "ELIGE TU NAVE", 48,
            new Vector2(0, 40), new Vector2(600, 70), TextAlignmentOptions.Center,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1));

        CreateTMPText(canvasObj.transform, "PointsText", "Puntos: 0", 28,
            new Vector2(-30, -30), new Vector2(300, 50), TextAlignmentOptions.TopRight,
            new Vector2(1, 1), new Vector2(1, 1));

        // Back button
        CreateButton(canvasObj.transform, "BackButton", "VOLVER", new Vector2(30, -30), new Vector2(150, 50),
            new Vector2(0, 1), new Vector2(0, 1));

        // Create ship card prefab
        CreateShipCardPrefab();

        // ScrollView for ship grid
        var scrollObj = new GameObject("ScrollView");
        scrollObj.transform.SetParent(canvasObj.transform, false);
        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        var scrollRt = scrollObj.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0.05f, 0.05f);
        scrollRt.anchorMax = new Vector2(0.95f, 0.85f);
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = Vector2.zero;
        scrollObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.3f);
        scrollObj.AddComponent<Mask>().showMaskGraphic = true;

        var contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollObj.transform, false);
        var contentRt = contentObj.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0, 1);
        contentRt.anchorMax = new Vector2(1, 1);
        contentRt.pivot = new Vector2(0.5f, 1);
        contentRt.sizeDelta = new Vector2(0, 1200);

        var grid = contentObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(350, 280);
        grid.spacing = new Vector2(15, 20);
        grid.padding = new RectOffset(15, 15, 15, 15);
        grid.childAlignment = TextAnchor.UpperCenter;

        var csf = contentObj.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRt;
        scrollRect.vertical = true;
        scrollRect.horizontal = false;

        // ShipSelectUI component
        var selectUI = canvasObj.AddComponent<ShipSelectUI>();
        selectUI.shipGrid = contentRt;
        selectUI.shipCardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ShipCard.prefab");
        selectUI.pointsText = canvasObj.transform.Find("PointsText").GetComponent<TextMeshProUGUI>();
        selectUI.backButton = canvasObj.transform.Find("BackButton").GetComponent<Button>();

        // EventSystem
        var eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/ShipSelect.unity");
        Debug.Log("ShipSelect scene created!");
    }

    static void CreateShipCardPrefab()
    {
        var cardObj = new GameObject("ShipCard");
        var cardRt = cardObj.AddComponent<RectTransform>();
        cardRt.sizeDelta = new Vector2(350, 280);

        var bg = cardObj.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.18f, 0.9f);

        // Border outline
        var outline = cardObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.5f, 0.8f, 0.5f);
        outline.effectDistance = new Vector2(2, 2);

        // Ship icon - larger and centered at top
        var iconObj = new GameObject("ShipIcon");
        iconObj.transform.SetParent(cardObj.transform, false);
        var iconRt = iconObj.AddComponent<RectTransform>();
        iconRt.anchoredPosition = new Vector2(0, 60);
        iconRt.sizeDelta = new Vector2(120, 120);
        var iconImg = iconObj.AddComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.color = Color.white;

        // Ship name - bigger, centered
        var nameObj = CreateTMPText(cardObj.transform, "ShipName", "Ship Name", 22,
            new Vector2(0, -12), new Vector2(300, 30), TextAlignmentOptions.Center);
        nameObj.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Stats panel
        CreateStatBarImproved(cardObj.transform, "SpeedBar", "VEL", new Vector2(0, -42), Color.green);
        CreateStatBarImproved(cardObj.transform, "FireRateBar", "CAD", new Vector2(0, -64), Color.cyan);
        CreateStatBarImproved(cardObj.transform, "DamageBar", "DAM", new Vector2(0, -86), new Color(1f, 0.4f, 0.2f));
        CreateStatBarImproved(cardObj.transform, "HealthBar", "VID", new Vector2(0, -108), Color.red);

        // Price text - prominent
        var priceObj = CreateTMPText(cardObj.transform, "PriceText", "GRATIS", 20,
            new Vector2(-60, -132), new Vector2(150, 30), TextAlignmentOptions.Center);
        priceObj.GetComponent<TextMeshProUGUI>().color = Color.yellow;

        // Action button - bigger
        CreateButton(cardObj.transform, "ActionButton", "ELEGIR", new Vector2(75, -132), new Vector2(130, 38));

        // Locked overlay
        var lockedObj = new GameObject("LockedOverlay");
        lockedObj.transform.SetParent(cardObj.transform, false);
        var lockedRt = lockedObj.AddComponent<RectTransform>();
        lockedRt.anchorMin = Vector2.zero;
        lockedRt.anchorMax = Vector2.one;
        lockedRt.offsetMin = Vector2.zero;
        lockedRt.offsetMax = Vector2.zero;
        lockedObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        CreateTMPText(lockedObj.transform, "LockedText", "BLOQUEADA", 28,
            Vector2.zero, new Vector2(200, 40));

        // ShipCard component
        var card = cardObj.AddComponent<ShipCard>();
        card.shipIcon = iconImg;
        card.shipNameText = cardObj.transform.Find("ShipName").GetComponent<TextMeshProUGUI>();
        card.speedBar = cardObj.transform.Find("SpeedBar/Slider").GetComponent<Slider>();
        card.fireRateBar = cardObj.transform.Find("FireRateBar/Slider").GetComponent<Slider>();
        card.damageBar = cardObj.transform.Find("DamageBar/Slider").GetComponent<Slider>();
        card.healthBar = cardObj.transform.Find("HealthBar/Slider").GetComponent<Slider>();
        card.speedValue = cardObj.transform.Find("SpeedBar/Value") != null ? cardObj.transform.Find("SpeedBar/Value").GetComponent<TextMeshProUGUI>() : null;
        card.fireRateValue = cardObj.transform.Find("FireRateBar/Value") != null ? cardObj.transform.Find("FireRateBar/Value").GetComponent<TextMeshProUGUI>() : null;
        card.damageValue = cardObj.transform.Find("DamageBar/Value") != null ? cardObj.transform.Find("DamageBar/Value").GetComponent<TextMeshProUGUI>() : null;
        card.healthValue = cardObj.transform.Find("HealthBar/Value") != null ? cardObj.transform.Find("HealthBar/Value").GetComponent<TextMeshProUGUI>() : null;
        card.priceText = cardObj.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        card.actionButton = cardObj.transform.Find("ActionButton").GetComponent<Button>();
        card.actionButtonText = cardObj.transform.Find("ActionButton/Text").GetComponent<TextMeshProUGUI>();
        card.cardBackground = bg;
        card.lockedOverlay = lockedObj;

        PrefabUtility.SaveAsPrefabAsset(cardObj, "Assets/Prefabs/ShipCard.prefab");
        DestroyImmediate(cardObj);
    }

    static void CreateStatBarImproved(Transform parent, string name, string label, Vector2 position, Color fillColor)
    {
        var container = new GameObject(name);
        container.transform.SetParent(parent, false);
        var rt = container.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(300, 20);

        // Label
        CreateTMPText(container.transform, "Label", label, 13, new Vector2(-130, 0), new Vector2(45, 18), TextAlignmentOptions.Left);

        // Slider
        var sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform, false);
        var sliderRt = sliderObj.AddComponent<RectTransform>();
        sliderRt.anchoredPosition = new Vector2(10, 0);
        sliderRt.sizeDelta = new Vector2(180, 10);
        var slider = sliderObj.AddComponent<Slider>();
        slider.interactable = false;
        slider.minValue = 0;
        slider.maxValue = 10;

        var slBg = new GameObject("Background");
        slBg.transform.SetParent(sliderObj.transform, false);
        var slBgRt = slBg.AddComponent<RectTransform>();
        slBgRt.anchorMin = Vector2.zero;
        slBgRt.anchorMax = Vector2.one;
        slBgRt.offsetMin = Vector2.zero;
        slBgRt.offsetMax = Vector2.zero;
        slBg.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 1f);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero;
        fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.offsetMin = Vector2.zero;
        fillAreaRt.offsetMax = Vector2.zero;

        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        var fillRt = fillObj.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0, 1);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fillObj.AddComponent<Image>().color = fillColor;

        slider.fillRect = fillRt;

        // Value text
        CreateTMPText(container.transform, "Value", "0", 13, new Vector2(125, 0), new Vector2(50, 18), TextAlignmentOptions.Right);
    }

    static void CreateStatBar(Transform parent, string name, string label, Vector2 position)
    {
        var container = new GameObject(name);
        container.transform.SetParent(parent, false);
        var rt = container.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(180, 25);

        CreateTMPText(container.transform, "Label", label, 12, new Vector2(-70, 0), new Vector2(40, 20));

        var sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform, false);
        var sliderRt = sliderObj.AddComponent<RectTransform>();
        sliderRt.anchoredPosition = new Vector2(20, 0);
        sliderRt.sizeDelta = new Vector2(120, 12);
        var slider = sliderObj.AddComponent<Slider>();
        slider.interactable = false;
        slider.minValue = 0;
        slider.maxValue = 10;

        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        var bgRt = bgObj.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bgObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero;
        fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.offsetMin = Vector2.zero;
        fillAreaRt.offsetMax = Vector2.zero;

        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        var fillRt = fillObj.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0, 1);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fillObj.AddComponent<Image>().color = Color.cyan;

        slider.fillRect = fillRt;
    }

    static void CreateGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/ScriptableObjects/GameConfig.asset");

        // Camera setup
        var cam = Camera.main;
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.08f);
        var shipCam = cam.gameObject.AddComponent<ShipCamera>();
        shipCam.follow_tightness = 5f;

        // Background with parallax
        var bg1 = new GameObject("Background1");
        var bgSr1 = bg1.AddComponent<SpriteRenderer>();
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/2D Space Kit/Backgrounds/1.png");
        if (bgSprite != null) bgSr1.sprite = bgSprite;
        bgSr1.sortingOrder = -10;
        bg1.transform.localScale = Vector3.one * 3f;
        bg1.transform.position = new Vector3(0, 0, 5);
        var pl1 = bg1.AddComponent<ParallaxLayer>();
        pl1.movement_resistance = 0.95f;

        // Spawn point
        var spawnObj = new GameObject("SpawnPoint");
        spawnObj.transform.position = Vector3.zero;

        // Managers
        var managersObj = new GameObject("Managers");

        var scoreManager = managersObj.AddComponent<ScoreManager>();
        var levelManager = managersObj.AddComponent<LevelManager>();
        var asteroidSpawner = managersObj.AddComponent<AsteroidSpawner>();
        var bossSpawner = managersObj.AddComponent<BossSpawner>();

        // Canvas
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // HUD
        var hudObj = new GameObject("HUD");
        hudObj.transform.SetParent(canvasObj.transform, false);
        var hudRt = hudObj.AddComponent<RectTransform>();
        hudRt.anchorMin = Vector2.zero;
        hudRt.anchorMax = Vector2.one;
        hudRt.offsetMin = Vector2.zero;
        hudRt.offsetMax = Vector2.zero;

        CreateTMPText(hudObj.transform, "ScoreText", "Puntos: 0", 28,
            new Vector2(30, -30), new Vector2(300, 50), TextAlignmentOptions.TopLeft,
            new Vector2(0, 1), new Vector2(0, 1));

        CreateTMPText(hudObj.transform, "LevelText", "Level 1", 32,
            new Vector2(0, -30), new Vector2(300, 50), TextAlignmentOptions.Top,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1));

        // Health bar
        var healthBarObj = CreateSlider(hudObj.transform, "HealthSlider",
            new Vector2(-30, -30), new Vector2(250, 25), Color.green,
            new Vector2(1, 1), new Vector2(1, 1));

        CreateTMPText(hudObj.transform, "HealthText", "HP: 100/100", 18,
            new Vector2(-30, -60), new Vector2(250, 30), TextAlignmentOptions.TopRight,
            new Vector2(1, 1), new Vector2(1, 1));

        var gameHud = hudObj.AddComponent<GameHUD>();
        gameHud.scoreText = hudObj.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        gameHud.levelText = hudObj.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        gameHud.healthSlider = healthBarObj.GetComponent<Slider>();
        gameHud.healthText = hudObj.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        gameHud.scoreManager = scoreManager;
        gameHud.levelManager = levelManager;

        // Boss health bar
        var bossHPPanel = new GameObject("BossHealthPanel");
        bossHPPanel.transform.SetParent(canvasObj.transform, false);
        var bossHPRt = bossHPPanel.AddComponent<RectTransform>();
        bossHPRt.anchorMin = new Vector2(0.15f, 0.88f);
        bossHPRt.anchorMax = new Vector2(0.85f, 0.95f);
        bossHPRt.offsetMin = Vector2.zero;
        bossHPRt.offsetMax = Vector2.zero;

        CreateTMPText(bossHPPanel.transform, "BossName", "BOSS", 24,
            new Vector2(0, 25), new Vector2(400, 35), TextAlignmentOptions.Center);

        var bossSlider = CreateSlider(bossHPPanel.transform, "BossHealthSlider",
            Vector2.zero, new Vector2(0, 0), Color.red, Vector2.zero, Vector2.one, true);

        var bossHB = bossHPPanel.AddComponent<BossHealthBar>();
        bossHB.panel = bossHPPanel;
        bossHB.healthSlider = bossSlider.GetComponent<Slider>();
        bossHB.bossNameText = bossHPPanel.transform.Find("BossName").GetComponent<TextMeshProUGUI>();

        // Level intro overlay
        var introObj = new GameObject("LevelIntroOverlay");
        introObj.transform.SetParent(canvasObj.transform, false);
        var introRt = introObj.AddComponent<RectTransform>();
        introRt.anchorMin = Vector2.zero;
        introRt.anchorMax = Vector2.one;
        introRt.offsetMin = Vector2.zero;
        introRt.offsetMax = Vector2.zero;
        var introGroup = introObj.AddComponent<CanvasGroup>();
        introGroup.alpha = 0;

        CreateTMPText(introObj.transform, "IntroText", "NIVEL 1", 80,
            new Vector2(0, 30), new Vector2(800, 120));
        introObj.transform.Find("IntroText").GetComponent<TextMeshProUGUI>().color = Color.yellow;

        CreateTMPText(introObj.transform, "SubText", "Destruye todos los asteroides!", 32,
            new Vector2(0, -50), new Vector2(800, 50));

        var introUI = introObj.AddComponent<LevelIntroUI>();
        introUI.levelIntroText = introObj.transform.Find("IntroText").GetComponent<TextMeshProUGUI>();
        introUI.subText = introObj.transform.Find("SubText").GetComponent<TextMeshProUGUI>();
        introUI.canvasGroup = introGroup;
        introUI.levelManager = levelManager;

        // Game Over panel
        var goPanel = new GameObject("GameOverPanel");
        goPanel.transform.SetParent(canvasObj.transform, false);
        var goPanelRt = goPanel.AddComponent<RectTransform>();
        goPanelRt.anchorMin = Vector2.zero;
        goPanelRt.anchorMax = Vector2.one;
        goPanelRt.offsetMin = Vector2.zero;
        goPanelRt.offsetMax = Vector2.zero;
        goPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f);

        CreateTMPText(goPanel.transform, "GameOverText", "FIN DEL JUEGO", 72,
            new Vector2(0, 100), new Vector2(800, 100));
        goPanel.transform.Find("GameOverText").GetComponent<TextMeshProUGUI>().color = Color.red;

        CreateTMPText(goPanel.transform, "FinalScoreText", "Puntos: 0", 36,
            new Vector2(0, 20), new Vector2(400, 50));

        CreateTMPText(goPanel.transform, "PointsEarnedText", "+0 points earned!", 24,
            new Vector2(0, -30), new Vector2(400, 40));

        CreateButton(goPanel.transform, "RestartButton", "REINICIAR", new Vector2(0, -100), new Vector2(250, 55));
        CreateButton(goPanel.transform, "MainMenuButton", "MENU", new Vector2(0, -170), new Vector2(250, 55));

        var goUI = goPanel.AddComponent<GameOverUI>();
        goUI.panel = goPanel;
        goUI.finalScoreText = goPanel.transform.Find("FinalScoreText").GetComponent<TextMeshProUGUI>();
        goUI.pointsEarnedText = goPanel.transform.Find("PointsEarnedText").GetComponent<TextMeshProUGUI>();
        goUI.restartButton = goPanel.transform.Find("RestartButton").GetComponent<Button>();
        goUI.mainMenuButton = goPanel.transform.Find("MainMenuButton").GetComponent<Button>();
        goUI.scoreManager = scoreManager;

        // Pause panel
        var pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);
        var pauseRt = pausePanel.AddComponent<RectTransform>();
        pauseRt.anchorMin = Vector2.zero;
        pauseRt.anchorMax = Vector2.one;
        pauseRt.offsetMin = Vector2.zero;
        pauseRt.offsetMax = Vector2.zero;
        pausePanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f);

        CreateTMPText(pausePanel.transform, "PausedText", "PAUSADO", 72,
            new Vector2(0, 80), new Vector2(600, 100));

        CreateButton(pausePanel.transform, "ResumeButton", "CONTINUAR", new Vector2(0, -20), new Vector2(250, 55));
        CreateButton(pausePanel.transform, "PauseMenuButton", "MENU", new Vector2(0, -90), new Vector2(250, 55));

        var pauseUI = pausePanel.AddComponent<PauseUI>();
        pauseUI.panel = pausePanel;
        pauseUI.resumeButton = pausePanel.transform.Find("ResumeButton").GetComponent<Button>();
        pauseUI.mainMenuButton = pausePanel.transform.Find("PauseMenuButton").GetComponent<Button>();

        // EventSystem
        var eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // GameSetup - wires everything
        var setupObj = new GameObject("GameSetup");
        var setup = setupObj.AddComponent<GameSetup>();
        setup.config = config;
        setup.inputActions = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        setup.spawnPoint = spawnObj.transform;
        setup.gameHUD = gameHud;
        setup.bossHealthBar = bossHB;
        setup.levelIntroUI = introUI;
        setup.gameOverUI = goUI;
        setup.scoreManager = scoreManager;
        setup.levelManager = levelManager;
        setup.asteroidSpawner = asteroidSpawner;
        setup.bossSpawner = bossSpawner;
        setup.shipCamera = shipCam;

        levelManager.asteroidSpawner = asteroidSpawner;
        levelManager.bossSpawner = bossSpawner;

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
        Debug.Log("Game scene created!");
    }

    static void SetupBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[] {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/ShipSelect.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("Build settings configured!");
    }

    static void SetupPhysicsLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        SetLayer(layers, 6, "Player");
        SetLayer(layers, 7, "Asteroid");
        SetLayer(layers, 8, "Boss");
        SetLayer(layers, 9, "PlayerProjectile");
        SetLayer(layers, 10, "EnemyProjectile");

        tagManager.ApplyModifiedProperties();

        // Collision matrix: only enable specific collisions
        // Disable all custom layer collisions first
        for (int i = 6; i <= 10; i++)
            for (int j = 6; j <= 10; j++)
                Physics2D.IgnoreLayerCollision(i, j, true);

        // Enable: Player <-> Asteroid
        Physics2D.IgnoreLayerCollision(6, 7, false);
        // Enable: Player <-> EnemyProjectile
        Physics2D.IgnoreLayerCollision(6, 10, false);
        // Enable: Asteroid <-> PlayerProjectile
        Physics2D.IgnoreLayerCollision(7, 9, false);
        // Enable: Boss <-> PlayerProjectile
        Physics2D.IgnoreLayerCollision(8, 9, false);

        Debug.Log("Physics layers and collision matrix configured!");
    }

    static void SetLayer(SerializedProperty layers, int index, string name)
    {
        var layer = layers.GetArrayElementAtIndex(index);
        if (string.IsNullOrEmpty(layer.stringValue))
            layer.stringValue = name;
    }

    // Helper methods
    static GameObject CreateTMPText(Transform parent, string name, string text, float fontSize,
        Vector2 position, Vector2 size, TextAlignmentOptions alignment = TextAlignmentOptions.Center,
        Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        if (anchorMin.HasValue) rt.anchorMin = anchorMin.Value;
        if (anchorMax.HasValue) rt.anchorMax = anchorMax.Value;

        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        return obj;
    }

    static GameObject CreateButton(Transform parent, string name, string text, Vector2 position, Vector2 size,
        Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        if (anchorMin.HasValue) rt.anchorMin = anchorMin.Value;
        if (anchorMax.HasValue) rt.anchorMax = anchorMax.Value;

        var img = obj.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.3f, 0.9f);

        var btn = obj.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.6f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.2f, 1f);
        btn.colors = colors;

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        var textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return obj;
    }

    static GameObject CreateSlider(Transform parent, string name, Vector2 position, Vector2 size,
        Color fillColor, Vector2? anchorMin = null, Vector2? anchorMax = null, bool stretch = false)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();

        if (stretch)
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(5, 5);
            rt.offsetMax = new Vector2(-5, -5);
        }
        else
        {
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            if (anchorMin.HasValue) rt.anchorMin = anchorMin.Value;
            if (anchorMax.HasValue) rt.anchorMax = anchorMax.Value;
        }

        var slider = obj.AddComponent<Slider>();
        slider.interactable = false;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;

        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(obj.transform, false);
        var bgRt = bgObj.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bgObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(obj.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero;
        fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.offsetMin = Vector2.zero;
        fillAreaRt.offsetMax = Vector2.zero;

        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        var fillRt = fillObj.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0, 1);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fillObj.AddComponent<Image>().color = fillColor;

        slider.fillRect = fillRt;

        return obj;
    }
}
