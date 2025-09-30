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

        // ����� ������
        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        // ������� ��� ������ (������ ���� Z ������)
        Rigidbody laserRb = laser.GetComponent<Rigidbody>();
        if (laserRb != null)
        {
            laserRb.linearVelocity = -firePoint.forward * laserSpeed; // ��� ��� ����
        }

        // ����� �����
        LaserProjectile laserProjectile = laser.GetComponent<LaserProjectile>();
        if (laserProjectile != null)
        {
            laserProjectile.damage = damagePerShot;
        }

        Destroy(laser, 3f);
    }
}