using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float fireRate = 3f;
    public float bulletDamage = 1f;
    public float bulletSpeed = 10f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    float fireCooldown;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = new Vector3(0, 0.8f, 0);
        }
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = 1f / fireRate;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        var b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = bulletDamage;
            b.speed = bulletSpeed;
        }

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void Setup(ShipData data, GameObject bulletPref)
    {
        fireRate = data.fireRate;
        bulletDamage = data.bulletDamage;
        bulletSpeed = data.bulletSpeed;
        bulletPrefab = bulletPref;
    }
}
