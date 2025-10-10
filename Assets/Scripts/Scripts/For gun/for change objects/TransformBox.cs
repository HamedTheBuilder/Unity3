using UnityEngine;
using System.Collections;  // ضروري للـ Coroutine

public class SwapOnHit : MonoBehaviour
{
    [Header("إعدادات التبديل")]
    public GameObject swapTarget; // الصندوق الثاني للتبديل

    [Header("خيارات إضافية")]
    public bool enableParticles = true;
    public ParticleSystem swapEffect;
    public AudioClip swapSound;
    public float swapVolume = 1.0f;  // تحكم في مستوى الصوت

    private bool isActive = true;
    private Vector3 myPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // حفظ المواقع الحالية
        myPosition = transform.position;
        if (swapTarget != null)
            targetPosition = swapTarget.transform.position;
    }

    // عند الاصطدام بالطلقة
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && isActive)
        {
            PerformSwap();
        }
    }

    // عند الدخول في Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && isActive)
        {
            PerformSwap();
        }
    }

    // دالة التبديل الرئيسية
    public void PerformSwap()
    {
        if (!isActive || swapTarget == null) return;

        SwapOnHit otherSwapScript = swapTarget.GetComponent<SwapOnHit>();
        if (otherSwapScript == null) return;

        // تبديل المواقع
        Vector3 currentMyPosition = transform.position;
        Vector3 currentTargetPosition = swapTarget.transform.position;

        transform.position = currentTargetPosition;
        swapTarget.transform.position = currentMyPosition;

        // تحديث المواقع المحفوظة
        myPosition = transform.position;
        targetPosition = swapTarget.transform.position;

        Debug.Log($"🔄 تم تبديل مواقع {name} و {swapTarget.name}");

        // تشغيل المؤثرات
        PlaySwapEffects();

        // تعطيل مؤقت لمنع التبديل السريع
        StartCoroutine(DisableTemporarily());
    }

    // تعطيل مؤقت لمنع التبديل السريع
    private IEnumerator DisableTemporarily()
    {
        isActive = false;
        Collider myCollider = GetComponent<Collider>();
        if (myCollider != null)
            myCollider.enabled = false;

        yield return new WaitForSeconds(0.3f);

        if (myCollider != null)
            myCollider.enabled = true;
        isActive = true;
    }

    // تشغيل المؤثرات البصرية والسمعية
    private void PlaySwapEffects()
    {
        // مؤثرات Particle
        if (enableParticles && swapEffect != null)
        {
            ParticleSystem effect1 = Instantiate(swapEffect, transform.position, Quaternion.identity);
            ParticleSystem effect2 = Instantiate(swapEffect, swapTarget.transform.position, Quaternion.identity);
            Destroy(effect1.gameObject, 2f);
            Destroy(effect2.gameObject, 2f);
        }

        // تشغيل الصوت بشكل موثوق
        if (swapSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = swapSound;
            tempSource.volume = swapVolume;
            tempSource.spatialBlend = 0f; // 2D sound
            tempSource.Play();

            Destroy(tempAudio, swapSound.length);
            Debug.Log($"🔊 تشغيل صوت التبديل: {swapSound.name}");
        }
        else
        {
            Debug.LogWarning("❌ الصوت غير معين!");
        }
    }

    // إعادة تعيين المواقع (مفيدة لإعادة اللعبة)
    public void ResetPositions()
    {
        transform.position = myPosition;
        if (swapTarget != null)
            swapTarget.transform.position = targetPosition;

        isActive = true;
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = true;
    }

    // عرض معلومات في Inspector
    void OnDrawGizmosSelected()
    {
        if (swapTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, swapTarget.transform.position);
            Gizmos.DrawWireCube(transform.position, GetComponent<Renderer>().bounds.size);
            Gizmos.DrawWireCube(swapTarget.transform.position, swapTarget.GetComponent<Renderer>().bounds.size);
            DrawArrow(transform.position, swapTarget.transform.position - transform.position, Color.green);
            DrawArrow(swapTarget.transform.position, transform.position - swapTarget.transform.position, Color.red);
        }
    }

    // دالة مساعدة لرسم الأسهم
    private void DrawArrow(Vector3 position, Vector3 direction, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(position, direction);
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(position + direction, right * 0.5f);
        Gizmos.DrawRay(position + direction, left * 0.5f);
    }
}
