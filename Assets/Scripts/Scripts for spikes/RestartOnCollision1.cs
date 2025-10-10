using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;           // Drag Player here
    public float speed = 5f;
    public float stopDistance = 2f;

    [Header("Animator & Audio")]
    public Animator animator;          // Enemy Animator
    public AudioSource audioSource;    // Enemy AudioSource
    public AudioClip screamClip;

    [Header("Player Settings")]
    public string playerTag = "Player";
    public MonoBehaviour playerMovementScript; // Player movement script to disable

    [Header("State")]
    public bool sleeping = true;       // Starts sleeping
    private bool isStopped = false;    // Enemy temporarily stopped
    private bool triggeredHit = false; // Player collision handled
    private bool isWalking = false;

    private void Start()
    {
        // Play sleep animation at the beginning
        if (animator != null)
            animator.SetBool("sleeping", sleeping);
    }

    private void Update()
    {
        if (player == null || sleeping || isStopped) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Move toward player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Rotate toward player
            transform.LookAt(player);

            // Animator walking
            if (!isWalking)
            {
                animator.SetBool("isWalking", true);
                isWalking = true;
            }
        }
        else
        {
            if (isWalking)
            {
                animator.SetBool("isWalking", false);
                isWalking = false;
            }
        }
    }

    // Called to stop enemy temporarily
    public void StopForSeconds(float duration)
    {
        if (!isStopped)
            StartCoroutine(StopRoutine(duration));
    }

    private IEnumerator StopRoutine(float duration)
    {
        isStopped = true;
        animator.SetBool("scream", true);

        if (audioSource != null && screamClip != null)
            audioSource.PlayOneShot(screamClip);

        // Force stop walking
        animator.SetBool("isWalking", false);
        isWalking = false;

        yield return new WaitForSeconds(duration);

        animator.SetBool("scream", false);
        isStopped = false;

        // Resume walking
        animator.SetBool("isWalking", true);
        isWalking = true;
    }

    // Handle Player collision
    private void OnCollisionEnter(Collision collision)
    {
        if (!triggeredHit && collision.collider.CompareTag(playerTag))
        {
            triggeredHit = true;

            // Stop player movement
            if (playerMovementScript != null)
                playerMovementScript.enabled = false;

            // Enemy hit animation
            animator.SetBool("didHit", true);

            // Restart scene after a short delay to let animation play
            StartCoroutine(RestartAfterDelay(0.5f)); // 1 second delay
        }
    }

    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Wake enemy (called from trigger)
    public void WakeUpEnemy()
    {
        sleeping = false;
        animator.SetBool("sleeping", false);
    }
}

