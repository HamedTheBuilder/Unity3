using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    [Header("🎵 إعدادات الصوت")]
    public AudioClip checkpointSound;
    private AudioSource audioSource;

    [Header("🎨 إعدادات الشكل")]
    public Color activatedColor = Color.yellow; // اللون بعد التفعيل
    private Renderer checkpointRenderer;
    private Color originalColor;

    private static HashSet<string> activatedCheckpoints = new HashSet<string>(); // 🔒 تشيك بوينتات مفعلة أثناء اللعب
    private bool activated = false;

    void Start()
    {
        // إعداد الصوت
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // إعداد الشكل
        checkpointRenderer = GetComponent<Renderer>();
        if (checkpointRenderer != null)
            originalColor = checkpointRenderer.material.color;

        // نتحقق إذا هذا التشيك بوينت مفعّل من قبل
        string id = gameObject.name;
        if (activatedCheckpoints.Contains(id))
        {
            ActivateVisualOnly(); // نغير اللون بدون صوت
            activated = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return; // إذا مفعّل مسبقًا، لا تكرار

        if (other.CompareTag("Player"))
        {
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
                respawn.SetCheckpoint(transform.position);

            // تشغيل الصوت لأول مرة فقط
            if (checkpointSound != null)
                audioSource.PlayOneShot(checkpointSound);

            // تغيير اللون
            ActivateVisualOnly();

            // نحفظ اسم التشيك بوينت حتى بعد إعادة تحميل المشهد
            activatedCheckpoints.Add(gameObject.name);
            activated = true;
        }
    }

    private void ActivateVisualOnly()
    {
        if (checkpointRenderer != null)
            checkpointRenderer.material.color = activatedColor;
    }
}
