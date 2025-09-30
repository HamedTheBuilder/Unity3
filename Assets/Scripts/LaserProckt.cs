using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [Header("Laser Properties")]
    public int damage = 1;
    public GameObject hitEffect;

    void OnTriggerEnter(Collider other)
    {
        // ������ ��� ��� ������� �� �����
        if (other.CompareTag("Asteroid"))
        {
            AsteroidHealth asteroidHealth = other.GetComponent<AsteroidHealth>();
            if (asteroidHealth != null)
            {
                // ����� ����� ��� �������
                asteroidHealth.TakeDamage(damage);

                // ����� ��������
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }

                // ����� ������
                Destroy(gameObject);
            }
        }
    }
}