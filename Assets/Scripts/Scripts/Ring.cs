using UnityEngine;

public class Ring : MonoBehaviour
{
    [Header("إعدادات الحلقة")]
    public int jumpsToAdd = 1;
    public float effectDuration = 1f;
    public ParticleSystem collectEffect;
    public AudioClip collectSound;

    [Header("إعدادات المؤقت المرئي")]
    public GameObject timerDisplay;
    public Renderer ringRenderer;
    public Color activeColor = Color.yellow;
    public Color inactiveColor = Color.white;

    [Header("إعدادات اللمعان")]
    public bool enableGlow = true;
    public Color glowColor = Color.yellow;
    public float glowIntensity = 2f;
    public bool pulseGlow = true;
    public float pulseSpeed = 2f;

    private bool isActive = true;
    private float resetTimer;
    private Material ringMaterial;
    private float originalGlowIntensity;

    // 🔒 لمنع الصوت والمؤثر من التكرار أثناء إعادة الظهور
    private bool collected = false;

    void Start()
    {
        InitializeGlow();
        ResetRing();
    }

    void InitializeGlow()
    {
        if (ringRenderer != null)
        {
            ringMaterial = ringRenderer.material;

            if (enableGlow)
            {
                ringMaterial.EnableKeyword("_EMISSION");
                ringMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
                originalGlowIntensity = glowIntensity;
            }
        }

        // إضافة AudioSource ثابت لتشغيل الصوت
        if (GetComponent<AudioSource>() == null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2D sound
        }
    }

    void Update()
    {
        // مؤقت إعادة الظهور
        if (!isActive)
        {
            resetTimer -= Time.deltaTime;

            if (timerDisplay != null)
            {
                timerDisplay.transform.LookAt(Camera.main.transform);
                TextMesh textMesh = timerDisplay.GetComponent<TextMesh>();
                if (textMesh != null)
                    textMesh.text = Mathf.CeilToInt(resetTimer).ToString();
            }

            if (ringRenderer != null)
                ringRenderer.material.color = Color.Lerp(activeColor, inactiveColor, resetTimer / effectDuration);

            if (resetTimer <= 0f)
                ResetRing();
        }
        else if (enableGlow && pulseGlow && ringMaterial != null)
        {
            HandlePulseGlow();
        }
    }

    void HandlePulseGlow()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float currentIntensity = originalGlowIntensity + pulse * (originalGlowIntensity * 0.5f);

        ringMaterial.SetColor("_EmissionColor", glowColor * currentIntensity);
        Color pulsedColor = Color.Lerp(activeColor, glowColor, pulse * 0.3f);
        ringMaterial.color = pulsedColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive && !collected)
        {
            BetterWallJump3D playerMovement = other.GetComponent<BetterWallJump3D>();
            if (playerMovement != null)
            {
                playerMovement.AddExtraJumps(jumpsToAdd);

                // تشغيل Particle
                if (collectEffect != null)
                    Instantiate(collectEffect, transform.position, Quaternion.identity);

                // تشغيل الصوت
                if (collectSound != null)
                {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    audioSource.clip = collectSound;
                    audioSource.Play();
                }

                DeactivateRing();

                collected = true;

                Debug.Log($"الحلقة تم جمعها! القفزات المضافة: {jumpsToAdd} - ستعود بعد {effectDuration} ثانية");
            }
        }
    }

    private void DeactivateRing()
    {
        isActive = false;
        resetTimer = effectDuration;

        if (ringRenderer != null)
            ringRenderer.enabled = false;

        if (timerDisplay != null)
            timerDisplay.SetActive(true);

        GetComponent<Collider>().enabled = false;

        if (enableGlow && ringMaterial != null)
            ringMaterial.SetColor("_EmissionColor", Color.black);
    }

    private void ResetRing()
    {
        isActive = true;
        collected = false;

        if (ringRenderer != null)
        {
            ringRenderer.enabled = true;
            ringRenderer.material.color = activeColor;
        }

        if (timerDisplay != null)
            timerDisplay.SetActive(false);

        GetComponent<Collider>().enabled = true;

        if (enableGlow && ringMaterial != null)
            ringMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    public void ChangeGlowColor(Color newGlowColor)
    {
        glowColor = newGlowColor;
        if (enableGlow && ringMaterial != null && isActive)
            ringMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    public void ChangeGlowIntensity(float newIntensity)
    {
        glowIntensity = newIntensity;
        originalGlowIntensity = newIntensity;
        if (enableGlow && ringMaterial != null && isActive)
            ringMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    void OnDrawGizmos()
    {
        if (!isActive)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.5f, "timer_icon");
        }
        else if (enableGlow)
        {
            Gizmos.color = glowColor;
            Gizmos.DrawWireSphere(transform.position, 1.2f);
        }
    }
}
