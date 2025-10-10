using UnityEngine;
using System.Collections;

public class BreakableBox : MonoBehaviour
{
    public enum BoxType
    {
        CoinBox,    // صندوق العملات
        ExplosiveBox // الصندوق المتفجر
    }

    [Header("Box Type")]
    public BoxType boxType = BoxType.CoinBox;

    [Header("Coin Settings")]
    public int minCoins = 3;
    public int maxCoins = 5;
    public GameObject coinPrefab;
    public float coinSpread = 1f;
    public float coinForce = 3f;

    [Header("Explosion Settings")]
    public float explosionForce = 15f;
    public float explosionRadius = 5f;
    public GameObject explosionEffect;

    [Header("Jump Boost")]
    public bool giveJumpBoost = true;
    public float jumpBoostForce = 15f;
    public float destroyDelay = 0.5f; // تأخير الاختفاء

    [Header("Audio")]
    public AudioClip breakSound;
    public AudioClip explosionSound;
    public AudioClip jumpBoostSound;

    private bool isBroken = false;
    private Animator animator;
    private Renderer boxRenderer;
    private Collider boxCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        boxRenderer = GetComponent<Renderer>();
        boxCollider = GetComponent<Collider>();
    }

    public void BreakBox()
    {
        if (isBroken) return;

        isBroken = true;

        // تعطيل الـ Collider فوراً
        if (boxCollider != null)
            boxCollider.enabled = false;

        switch (boxType)
        {
            case BoxType.CoinBox:
                ReleaseCoins();
                if (giveJumpBoost)
                {
                    ApplyJumpBoost();
                }
                PlayBreakSound();
                break;

            case BoxType.ExplosiveBox:
                StartCoroutine(ExplodeWithDelay());
                break;
        }

        // Play break animation
        if (animator != null)
            animator.SetTrigger("Break");

        // تدمير الكائن بعد delay
        StartCoroutine(DestroyAfterDelay());
    }

    void ReleaseCoins()
    {
        if (coinPrefab == null) return;

        int coinCount = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coinCount; i++)
        {
            StartCoroutine(SpawnCoinWithDelay(i * 0.1f));
        }
    }

    IEnumerator SpawnCoinWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (coinPrefab != null)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Rigidbody coinRb = coin.GetComponent<Rigidbody>();
            if (coinRb != null)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-coinSpread, coinSpread),
                    Random.Range(0.8f, 1.2f),
                    Random.Range(-coinSpread, coinSpread)
                ).normalized;

                coinRb.AddForce(randomDirection * coinForce, ForceMode.Impulse);
                coinRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }

    IEnumerator ExplodeWithDelay()
    {
        // انتظار بسيط قبل الانفجار للأنيميشن
        yield return new WaitForSeconds(0.2f);

        Explode();
    }

    void Explode()
    {
        // Create explosion effect
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Apply explosion force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 3f);

                // Damage player if nearby
                if (hit.CompareTag("Player"))
                {
                    PlayerMoveBridge player = hit.GetComponent<PlayerMoveBridge>();
                    if (player != null)
                    {
                        player.TakeDamage(1);
                    }
                }

                // دفع الأعداء بقوة
                if (hit.CompareTag("Enemy"))
                {
                    GoatEnemy enemy = hit.GetComponent<GoatEnemy>();
                    if (enemy != null)
                    {
                        Vector3 direction = (hit.transform.position - transform.position).normalized;
                        enemy.Knockback(direction, explosionForce * 1.5f);
                    }
                }
            }
        }

        PlayExplosionSound();
    }

    void ApplyJumpBoost()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody playerRb = hit.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
                    playerRb.AddForce(Vector3.up * jumpBoostForce, ForceMode.Impulse);

                    PlayJumpBoostSound();
                }
            }
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (gameObject != null)
            Destroy(gameObject);
    }

    void PlayBreakSound()
    {
        if (breakSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("BoxBreak");
    }

    void PlayExplosionSound()
    {
        if (explosionSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Explosion");
    }

    void PlayJumpBoostSound()
    {
        if (jumpBoostSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("JumpBoost");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsPlayerAbove(collision.gameObject))
        {
            BreakBox();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsPlayerAbove(other.gameObject))
        {
            BreakBox();
        }
    }

    bool IsPlayerAbove(GameObject player)
    {
        return player.transform.position.y > transform.position.y;
    }

    // لرؤية منطقة الانفجار في المحرر
    void OnDrawGizmosSelected()
    {
        if (boxType == BoxType.ExplosiveBox)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}