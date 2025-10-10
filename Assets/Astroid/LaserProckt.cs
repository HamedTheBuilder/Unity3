using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [Header("Laser Properties")]
    public int damage = 1;
    public GameObject hitEffect;

    void OnTriggerEnter(Collider other)
    {
        // «· Õﬁﬁ ≈–« ﬂ«‰ «· ’«œ„ „⁄ ﬂÊÌﬂ»
        if (other.CompareTag("Asteroid"))
        {
            AsteroidHealth asteroidHealth = other.GetComponent<AsteroidHealth>();
            if (asteroidHealth != null)
            {
                //  ÿ»Ìﬁ «·÷—— ⁄·Ï «·ﬂÊÌﬂ»
                asteroidHealth.TakeDamage(damage);

                //  √ÀÌ— «·«’ÿœ«„
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }

                //  œ„Ì— «··Ì“—
                Destroy(gameObject);
            }
        }
    }
}