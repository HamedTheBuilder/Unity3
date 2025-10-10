using UnityEngine;
using System.Collections;

public class GoatEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    public int damage = 1;

    [Header("Player Bounce")]
    public float bounceForce = 10f;
    public float playerBounceForce = 8f;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1f;
    public float destroyDelay = 0.5f;

    [Header("Knockback Settings")]
    public float attackKnockbackForce = 20f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingForward = true;
    private Rigidbody rb;
    private Animator animator;
    private bool isDead = false;
    private bool canDamagePlayer = true;
    private float lastDamageTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        startPosition = transform.position;
        targetPosition = startPosition + Vector3.forward * moveDistance;
        canDamagePlayer = true;
        lastDamageTime = -damageCooldown;

        // تأكد من التاج
        if (!gameObject.CompareTag("Enemy"))
        {
            Debug.LogWarning("Goat enemy should have 'Enemy' tag! Current tag: " + gameObject.tag);
        }

        // تحقق من وجود الكولايدر
        Collider existingCollider = GetComponent<Collider>();
        if (existingCollider == null)
        {
            Debug.LogError("No collider found on goat! Please add a collider component.");
        }
        else
        {
            Debug.Log("Goat has collider: " + existingCollider.GetType().Name);
        }
    }

    void Update()
    {
        if (!isDead)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (movingForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                movingForward = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                movingForward = true;
            }
        }

        Vector3 direction = movingForward ? Vector3.forward : Vector3.back;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject, collision.GetContact(0).point);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject, collision.GetContact(0).point);
        }
    }

    void HandlePlayerCollision(GameObject player, Vector3 contactPoint)
    {
        if (isDead) return;

        PlayerMoveBridge playerScript = player.GetComponent<PlayerMoveBridge>();
        if (playerScript != null)
        {
            if (IsPlayerAbove(player))
            {
                DieFromJump();

                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
                    playerRb.AddForce(Vector3.up * playerBounceForce, ForceMode.Impulse);
                }
            }
            else if (canDamagePlayer && Time.time >= lastDamageTime + damageCooldown)
            {
                playerScript.TakeDamage(damage);
                PushPlayerAway(player, contactPoint);

                lastDamageTime = Time.time;
                StartCoroutine(DamageCooldown());
            }
        }
    }

    bool IsPlayerAbove(GameObject player)
    {
        float heightDifference = player.transform.position.y - transform.position.y;
        bool isAbove = heightDifference > 0.3f;
        return isAbove;
    }

    void PushPlayerAway(GameObject player, Vector3 contactPoint)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 pushDirection = (player.transform.position - contactPoint).normalized;
            pushDirection.y = 0.3f;

            playerRb.AddForce(pushDirection * bounceForce, ForceMode.Impulse);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("GoatPush");
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamagePlayer = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamagePlayer = true;
    }

    public void DieFromJump()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        DisableComponents();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("GoatDeath");
            AudioManager.Instance.PlaySound("PlayerBounce");
        }

        StartCoroutine(DestroyAfterDelay());
    }

    public void OnAttacked()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        DisableComponents();

        // دفع قوي عند الضرب
        if (rb != null)
        {
            PlayerMoveBridge player = FindAnyObjectByType<PlayerMoveBridge>();
            if (player != null)
            {
                Vector3 knockbackDirection = (transform.position - player.transform.position).normalized;
                knockbackDirection.y = 0.5f;
                rb.AddForce(knockbackDirection * attackKnockbackForce, ForceMode.Impulse);
            }
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("GoatDeath");

        StartCoroutine(DestroyAfterDelay());
    }

    void DisableComponents()
    {
        // تعطيل الـ Collider الموجود على الشخصية
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        // تعطيل السكربت نفسه
        enabled = false;

        // إيقاف الحركة
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (gameObject != null)
            Destroy(gameObject);
    }

    public void Knockback(Vector3 direction, float force)
    {
        if (rb != null && !isDead)
        {
            direction.y = 0.3f;
            rb.AddForce(direction * force, ForceMode.Impulse);
            StartCoroutine(StopMovementTemporarily());
        }
    }

    IEnumerator StopMovementTemporarily()
    {
        float originalSpeed = moveSpeed;
        moveSpeed = 0f;
        yield return new WaitForSeconds(1f);

        if (!isDead)
        {
            moveSpeed = originalSpeed;
        }
    }
}