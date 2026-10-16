using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;
    bool invulnerable;
    float invulnerabilityDuration = 1.5f;
    float invulnerabilityTimer;
    SpriteRenderer spriteRenderer;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDeath;

    public float CurrentHealth => currentHealth;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        if (!invulnerable) return;

        invulnerabilityTimer -= Time.deltaTime;
        if (spriteRenderer != null)
        {
            float alpha = Mathf.PingPong(Time.time * 10f, 1f) > 0.5f ? 1f : 0.3f;
            var c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        if (invulnerabilityTimer <= 0f)
        {
            invulnerable = false;
            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (invulnerable) return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            OnPlayerDeath?.Invoke();
            return;
        }

        invulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
    }

    public void Setup(ShipData data)
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
