using UnityEngine;

[CreateAssetMenu(fileName = "NewShip", menuName = "AsteroidCommand/Ship Data")]
public class ShipData : ScriptableObject
{
    public string shipName;
    public int shipIndex;
    public GameObject prefab;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Stats")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 300f;
    public float fireRate = 3f;
    public float bulletDamage = 1f;
    public float bulletSpeed = 10f;
    public float maxHealth = 100f;

    [Header("Unlock")]
    public int unlockCost;
}
