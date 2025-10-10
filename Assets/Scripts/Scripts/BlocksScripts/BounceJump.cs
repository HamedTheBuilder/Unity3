using UnityEngine;

public class BounceCube : MonoBehaviour
{
    public float bounceForce = 25f;
    public int maxSteps = 5;
    public AudioClip bounceSound;
    public float respawnTime = 3f;

    private int stepCount = 0;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
                playerRb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }

            // تشغيل الصوت
            if (bounceSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(bounceSound);
            }

            // زيادة العد
            stepCount++;

            // إذا وصل للحد الأقصى
            if (stepCount >= maxSteps)
            {
                // نجعل الصوت يشتغل أولاً ثم نجعل الكائن inactive بعد وقت صغير
                float disableDelay = (bounceSound != null) ? bounceSound.length : 0.01f;
                Invoke(nameof(DeactivateCube), disableDelay);
            }
        }
    }

    void DeactivateCube()
    {
        gameObject.SetActive(false);

        // إعادة التنشيط بعد فترة
        if (respawnTime > 0f)
            Invoke(nameof(ReactivateCube), respawnTime);
    }

    void ReactivateCube()
    {
        stepCount = 0;
        gameObject.SetActive(true);
    }
}
