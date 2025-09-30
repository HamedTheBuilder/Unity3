using UnityEngine;
using System.Collections;

public class AsteroidHealth : MonoBehaviour
{
    [Header("Asteroid Health")]
    public int maxHealth = 5;
    public int currentHealth; // €Ì—Â« ≈·Ï public
    public GameObject destructionEffect;
    public int scoreValue = 10;

    [Header("Damage Effects")]
    public Color damageColor = Color.red;
    public float colorChangeDuration = 0.5f;

    [Header("Sound Effects")]
    public AudioClip explosionSound;
    public float soundVolume = 1f;

    private Renderer asteroidRenderer;
    private Color originalColor;
    private AudioSource audioSource;
    private bool isDestroyed = false;

    void Start()
    {
        currentHealth = maxHealth;
        asteroidRenderer = GetComponent<Renderer>();

        if (asteroidRenderer != null)
        {
            originalColor = asteroidRenderer.material.color;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        //  €ÌÌ— ·Ê‰ «·ﬂÊÌﬂ» ≈·Ï «·√Õ„—
        StartCoroutine(ChangeAsteroidColor());

        if (currentHealth <= 0)
        {
            DestroyAsteroid();
        }
    }

    IEnumerator ChangeAsteroidColor()
    {
        if (asteroidRenderer != null)
        {
            asteroidRenderer.material.color = damageColor;
            yield return new WaitForSeconds(colorChangeDuration);

            if (!isDestroyed && asteroidRenderer != null)
            {
                asteroidRenderer.material.color = originalColor;
            }
        }
    }

    void DestroyAsteroid()
    {
        isDestroyed = true;

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, soundVolume);
        }

        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }
}