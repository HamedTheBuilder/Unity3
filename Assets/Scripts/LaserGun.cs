using UnityEngine;

public class SimpleLaserGun : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public float laserSpeed = 20f;
    public float fireRate = 0.2f;
    public int damagePerShot = 1;

    private float nextFireTime = 0f;

    void Start()
    {
        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            ShootLaser();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootLaser()
    {
        if (laserPrefab == null || firePoint == null) return;

        // ≈‰‘«¡ «··Ì“—
        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        // «·≈ÿ·«ﬁ ≈·Ï «·√„«„ (»« Ã«Â „ÕÊ— Z «·„ÊÃ»)
        Rigidbody laserRb = laser.GetComponent<Rigidbody>();
        if (laserRb != null)
        {
            laserRb.linearVelocity = -firePoint.forward * laserSpeed; // €Ì— ≈·Ï ”«·»
        }

        // ≈⁄œ«œ «·÷——
        LaserProjectile laserProjectile = laser.GetComponent<LaserProjectile>();
        if (laserProjectile != null)
        {
            laserProjectile.damage = damagePerShot;
        }

        Destroy(laser, 3f);
    }
}