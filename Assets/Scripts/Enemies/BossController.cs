using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    public float maxHealth = 200f;
    float currentHealth;
    float moveSpeed;
    float fireRate;
    float bulletDamage;
    float bulletSpeed;
    int bulletCount;
    float bulletSpreadAngle;
    Color bulletColor;
    bool hasShield;
    float shieldDuration;
    float shieldCooldown;
    BossMovementPattern movementPattern;
    float orbitRadius;
    float chargeSpeed;
    int pointsReward;
    GameObject bulletPrefab;

    float fireCooldown;
    float shieldTimer;
    float shieldCooldownTimer;
    bool shieldActive;
    Transform player;
    float movementTimer;
    Vector2 moveTarget;
    SpriteRenderer spriteRenderer;
    GameObject shieldVisual;

    public event System.Action<float, float> OnHealthChanged;
    public event System.Action OnBossDeath;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public int PointsReward => pointsReward;

    public void Setup(BossData data, GameObject bulletPref, Transform playerTransform)
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        moveSpeed = data.moveSpeed;
        fireRate = data.fireRate;
        bulletDamage = data.bulletDamage;
        bulletSpeed = data.bulletSpeed;
        bulletCount = Mathf.Max(1, data.bulletCount);
        bulletSpreadAngle = data.bulletSpreadAngle;
        bulletColor = data.bulletColor;
        hasShield = data.hasShield;
        shieldDuration = data.shieldDuration;
        shieldCooldown = data.shieldCooldown;
        movementPattern = data.movementPattern;
        orbitRadius = data.orbitRadius;
        chargeSpeed = data.chargeSpeed;
        pointsReward = data.pointsReward;
        bulletPrefab = bulletPref;
        player = playerTransform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = data.sprite;

        if (hasShield)
            CreateShieldVisual();
    }

    public void ScaleStats(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth;
        bulletDamage *= multiplier;
        fireRate *= Mathf.Min(multiplier, 1.5f);
    }

    void CreateShieldVisual()
    {
        shieldVisual = new GameObject("Shield");
        shieldVisual.transform.SetParent(transform);
        shieldVisual.transform.localPosition = Vector3.zero;
        var sr = shieldVisual.AddComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        sr.color = new Color(0.3f, 0.6f, 1f, 0.3f);
        sr.sortingOrder = 1;
        shieldVisual.transform.localScale = Vector3.one * 1.3f;
        shieldVisual.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        UpdateMovement();
        UpdateShooting();
        UpdateShield();
    }

    void UpdateMovement()
    {
        switch (movementPattern)
        {
            case BossMovementPattern.Orbit:
                movementTimer += Time.deltaTime * moveSpeed;
                Vector2 orbitPos = (Vector2)player.position + new Vector2(
                    Mathf.Cos(movementTimer) * orbitRadius,
                    Mathf.Sin(movementTimer) * orbitRadius
                );
                transform.position = Vector2.MoveTowards(transform.position, orbitPos, moveSpeed * Time.deltaTime * 2f);
                break;

            case BossMovementPattern.Strafe:
                movementTimer += Time.deltaTime;
                if (movementTimer > 2f)
                {
                    movementTimer = 0f;
                    moveTarget = new Vector2(
                        Random.Range(-7f, 7f),
                        Random.Range(1f, 4f)
                    );
                }
                transform.position = Vector2.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
                break;

            case BossMovementPattern.ChargeAndRetreat:
                movementTimer += Time.deltaTime;
                if (movementTimer < 2f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.position, chargeSpeed * Time.deltaTime);
                }
                else if (movementTimer < 4f)
                {
                    Vector2 retreatPos = (Vector2)player.position + ((Vector2)transform.position - (Vector2)player.position).normalized * 6f;
                    transform.position = Vector2.MoveTowards(transform.position, retreatPos, moveSpeed * Time.deltaTime);
                }
                else
                {
                    movementTimer = 0f;
                }
                break;

            case BossMovementPattern.Stationary:
                break;
        }

        if (player != null)
        {
            Vector2 dir = (Vector2)player.position - (Vector2)transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void UpdateShooting()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            FireBullets();
            fireCooldown = 1f / fireRate;
        }
    }

    void FireBullets()
    {
        if (bulletPrefab == null) return;

        float baseAngle = transform.eulerAngles.z;

        for (int i = 0; i < bulletCount; i++)
        {
            float offset = 0f;
            if (bulletCount > 1)
                offset = Mathf.Lerp(-bulletSpreadAngle / 2f, bulletSpreadAngle / 2f, (float)i / (bulletCount - 1));

            Quaternion rot = Quaternion.Euler(0, 0, baseAngle + offset);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rot);

            var bb = bullet.GetComponent<BossBullet>();
            if (bb != null)
            {
                bb.damage = bulletDamage;
                bb.speed = bulletSpeed;
            }

            var sr = bullet.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = bulletColor;
        }
    }

    void UpdateShield()
    {
        if (!hasShield) return;

        if (shieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                shieldActive = false;
                shieldCooldownTimer = shieldCooldown;
                if (shieldVisual != null) shieldVisual.SetActive(false);
            }
        }
        else
        {
            shieldCooldownTimer -= Time.deltaTime;
            if (shieldCooldownTimer <= 0f)
            {
                shieldActive = true;
                shieldTimer = shieldDuration;
                if (shieldVisual != null) shieldVisual.SetActive(true);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (shieldActive)
            damage *= 0.2f;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (spriteRenderer != null)
            StartCoroutine(FlashDamage());

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            OnBossDeath?.Invoke();
            Destroy(gameObject, 0.1f);
        }
    }

    System.Collections.IEnumerator FlashDamage()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
