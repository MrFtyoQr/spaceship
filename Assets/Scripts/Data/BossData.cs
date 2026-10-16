using UnityEngine;

[CreateAssetMenu(fileName = "NewBoss", menuName = "AsteroidCommand/Boss Data")]
public class BossData : ScriptableObject
{
    public string bossName;
    public Sprite sprite;

    [Header("Stats")]
    public float maxHealth = 200f;
    public float moveSpeed = 2f;
    public float fireRate = 1f;
    public float bulletDamage = 10f;
    public float bulletSpeed = 6f;
    public int pointsReward = 500;

    [Header("Abilities")]
    public bool hasShield;
    public float shieldDuration = 3f;
    public float shieldCooldown = 8f;
    public int bulletCount = 1;
    public float bulletSpreadAngle;
    public Color bulletColor = Color.red;

    [Header("Movement")]
    public BossMovementPattern movementPattern;
    public float orbitRadius = 3f;
    public float chargeSpeed = 8f;
}

public enum BossMovementPattern
{
    Orbit,
    Strafe,
    ChargeAndRetreat,
    Stationary
}
