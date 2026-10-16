using UnityEngine;
using System;

public class AsteroidController : MonoBehaviour
{
    public float health = 1f;
    public int pointsValue = 50;
    public AsteroidSize size = AsteroidSize.Med;

    public static event Action<int> OnAsteroidDestroyed;

    public void SetVelocity(Vector2 velocity)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = velocity;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            OnAsteroidDestroyed?.Invoke(pointsValue);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(20f);
            OnAsteroidDestroyed?.Invoke(0);
            Destroy(gameObject);
        }
    }
}

public enum AsteroidSize
{
    Small,
    Med,
    Big,
    Huge
}
