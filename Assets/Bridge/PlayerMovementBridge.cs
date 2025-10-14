using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMoveBridge : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 7f;
    public int maxJumps = 2;

    [Header("Player Stats")]
    public int maxHealth = 3;
    public int coins = 0;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackRadius = 1.5f;
    public int attackDamage = 1;
    public float attackKnockback = 10f;

    [Header("Bounce Settings")]
    public float enemyBounceForce = 8f;
    public float enemySidePushForce = 10f;

    [Header("Fall Death Settings")]
    public float deathYLevel = -10f;
    public float checkFallInterval = 0.5f;

    [Header("Respawn Settings")]
    public float respawnDelay = 1f;

    [Header("References")]
    public Transform cameraTransform;
    public PlayerAnimationController animationController;
    public LayerMask attackableLayers;

    // Private variables
    private Rigidbody rb;
    private int currentHealth;
    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private Vector3 checkpointPosition;
    private Quaternion checkpointRotation;
    private CameraFollow cameraFollow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        checkpointPosition = transform.position;
        checkpointRotation = transform.rotation;
        cameraFollow = FindObjectOfType<CameraFollow>();

        if (attackableLayers == 0)
            attackableLayers = LayerMask.GetMask("Default");

        StartCoroutine(CheckFallDeath());
        UpdateHealthUI();
        UpdateCoinUI();
    }

    void Update()
    {
        if (isDead) return;

        GetInput();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleSprint();
        HandleAttack();
    }

    void FixedUpdate()
    {
        CheckGrounded();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");
        crouchInput = Input.GetKeyDown(KeyCode.C);
        sprintInput = Input.GetKey(KeyCode.LeftShift);
        attackInput = Input.GetMouseButtonDown(0);
    }

    void HandleMovement()
    {
        if (cameraTransform == null) return;

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirection = (verticalInput * cameraForward + horizontalInput * cameraTransform.right).normalized;

        float currentSpeed = walkSpeed;
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isSprinting && verticalInput > 0.1f)
            currentSpeed = sprintSpeed;

        if (rb != null)
        {
            Vector3 targetVelocity = moveDirection * currentSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }

        if (animationController != null)
        {
            bool isMoving = moveDirection.magnitude > 0.1f;
            animationController.SetMovement(currentSpeed, moveDirection, isMoving);
        }
    }

    void HandleJump()
    {
        if (jumpInput && jumpCount < maxJumps)
        {
            PerformJump();
        }
    }

    void PerformJump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jumpCount++;
        if (animationController != null)
        {
            animationController.Jump(jumpCount);
        }
    }

    void HandleCrouch()
    {
        if (crouchInput)
        {
            isCrouching = !isCrouching;
            isSprinting = false;

            if (animationController != null)
            {
                animationController.SetCrouching(isCrouching);
            }
        }
    }

    void HandleSprint()
    {
        if (!isCrouching)
        {
            isSprinting = sprintInput && verticalInput > 0.1f;
        }
        else
        {
            isSprinting = false;
        }
    }

    void HandleAttack()
    {
        if (attackInput && !isAttacking)
        {
            if (animationController != null)
            {
                animationController.Attack();
                isAttacking = true;
                StartCoroutine(ResetAttackState(0.5f));
            }
        }
    }

    IEnumerator ResetAttackState(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        float rayDistance = 0.2f;
        LayerMask groundLayer = ~0;

        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            if (animationController != null)
            {
                animationController.Land();
            }
        }

        if (animationController != null)
        {
            animationController.SetGrounded(isGrounded);
        }
    }

    IEnumerator CheckFallDeath()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkFallInterval);

            if (!isDead && transform.position.y < deathYLevel)
            {
                DieFromFall();
            }
        }
    }

    public void DieFromFall()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died from fall!");

        if (cameraFollow != null)
        {
            cameraFollow.StopFollowing();
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (cameraFollow != null)
        {
            cameraFollow.StopFollowing();
        }

        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }

    void Respawn()
    {
        transform.position = checkpointPosition;
        transform.rotation = checkpointRotation;

        currentHealth = maxHealth;
        isDead = false;
        jumpCount = 0;
        isCrouching = false;
        isSprinting = false;
        isAttacking = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        if (cameraFollow != null)
        {
            cameraFollow.ResumeFollowing();
        }

        UpdateHealthUI();
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        UpdateCoinUI();
    }

    // ÏÇáÉ æÇÍÏÉ ÝÞØ á SetCheckpoint - åÐÇ åæ ÇáÍá
    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        checkpointRotation = transform.rotation;
        Debug.Log($"Checkpoint set to: {position}");
    }

    void UpdateHealthUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHearts(currentHealth);
        }
    }

    void UpdateCoinUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoinDisplay(coins);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision.gameObject, collision.GetContact(0).point);
        }
    }



    void HandleEnemyCollision(GameObject enemy, Vector3 contactPoint)
    {
        if (isDead) return;

        GoatEnemy enemyScript = enemy.GetComponent<GoatEnemy>();
        if (enemyScript != null)
        {
            if (IsAboveEnemy(enemy))
            {
                BounceOnEnemy();
                enemyScript.DieFromJump();
            }
            else
            {
                TakeDamage(1);
                PushPlayerFromEnemy(contactPoint);
            }
        }
    }

    bool IsAboveEnemy(GameObject enemy)
    {
        float heightDifference = transform.position.y - enemy.transform.position.y;
        return heightDifference > 0.5f && rb.linearVelocity.y < 0;
    }

    void BounceOnEnemy()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * enemyBounceForce, ForceMode.Impulse);
        }
    }

    void PushPlayerFromEnemy(Vector3 contactPoint)
    {
        if (rb != null)
        {
            Vector3 pushDirection = (transform.position - contactPoint).normalized;
            pushDirection.y = 0.3f;
            rb.AddForce(pushDirection * enemySidePushForce, ForceMode.Impulse);
        }
    }

    // ÅÖÇÝÉ ÇáãÊÛíÑÇÊ ÇáãÝÞæÏÉ
    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;
    private bool crouchInput;
    private bool sprintInput;
    private bool attackInput;

    // Public methods
    public bool IsDead() { return isDead; }
    public int GetCurrentHealth() { return currentHealth; }
    public int GetCoins() { return coins; }
    public bool IsGrounded() { return isGrounded; }
    public bool IsCrouching() { return isCrouching; }
    public bool IsSprinting() { return isSprinting; }
    public bool IsAttacking() { return isAttacking; }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackPosition = transform.position + transform.forward * attackRange;
        Gizmos.DrawWireSphere(attackPosition, attackRadius);
        Gizmos.DrawLine(transform.position, attackPosition);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * 0.2f);

        Gizmos.color = Color.yellow;
        Vector3 deathLineStart = new Vector3(transform.position.x - 5f, deathYLevel, transform.position.z);
        Vector3 deathLineEnd = new Vector3(transform.position.x + 5f, deathYLevel, transform.position.z);
        Gizmos.DrawLine(deathLineStart, deathLineEnd);
    }
}