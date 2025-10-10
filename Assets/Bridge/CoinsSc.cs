using UnityEngine;
using System.Collections;
using TMPro;

public class CoinsSc : MonoBehaviour
{
    [Header("Coin Value")]
    public int value = 1;

    [Header("Floating Animation")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;
    public float rotationSpeed = 100f;

    [Header("Fly To UI Settings")]
    public float flyToUISpeed = 1f;
    public float flyCurveHeight = 2f;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private bool collected = false;
    private bool isFlyingToUI = false;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
        StartCoroutine(FloatAnimation());
    }

    IEnumerator FloatAnimation()
    {
        while (!collected && !isFlyingToUI)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected && !isFlyingToUI)
        {
            collected = true;
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        AddCoinToPlayer();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("CoinCollect");

        StartCoroutine(FlyToExactCorner());
    }

    void AddCoinToPlayer()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.AddCoins(value);
        }
        else
        {
            PlayerMoveBridge player = FindAnyObjectByType<PlayerMoveBridge>();
            if (player != null)
            {
                player.AddCoin(value);
            }
        }
    }

    IEnumerator FlyToExactCorner()
    {
        isFlyingToUI = true;

        // تعطيل المكونات
        GetComponent<Collider>().enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Vector3 startPos = transform.position;

        // الحصول على الموقع الدقيق للزاوية اليمنى العليا
        Vector3 exactCorner = GetExactTopRightCorner();

        float distance = Vector3.Distance(startPos, exactCorner);
        float duration = distance / flyToUISpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (this == null) yield break;

            float progress = elapsed / duration;

            // حركة منحنية
            Vector3 currentPos = CalculateBezierCurve(startPos, exactCorner, progress);
            transform.position = currentPos;

            // تصغير تدريجي يبدأ من منتصف الرحلة
            if (progress > 0.4f)
            {
                float scaleProgress = (progress - 0.4f) / 0.6f;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, scaleProgress);
            }

            transform.Rotate(Vector3.up, 600 * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // التأكد من الوصول للهدف بالضبط
        transform.position = exactCorner;
        transform.localScale = Vector3.zero;

        Destroy(gameObject);
    }

    Vector3 GetExactTopRightCorner()
    {
        // إذا كان هناك UIManager ونص العملات، استخدم موقعه
        if (UIManager.Instance != null && UIManager.Instance.coinText != null)
        {
            return GetExactUIElementPosition(UIManager.Instance.coinText.GetComponent<RectTransform>());
        }

        // إذا لم يكن هناك UI، استخدم زاوية الشاشة مباشرة
        return GetScreenTopRightCorner();
    }

    Vector3 GetExactUIElementPosition(RectTransform uiElement)
    {
        Canvas canvas = uiElement.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // للـ Screen Space Overlay
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiElement.position);
            return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5f));
        }
        else
        {
            // للـ Screen Space Camera
            Vector3[] worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            return worldCorners[2]; // الزاوية اليمنى العليا
        }
    }

    Vector3 GetScreenTopRightCorner()
    {
        // الزاوية اليمنى العليا للشاشة بالضبط
        float x = Screen.width - 10f; // 10 بكسلات من الحافة
        float y = Screen.height - 10f; // 10 بكسلات من الحافة
        float z = 5f; // مسافة من الكاميرا

        return Camera.main.ScreenToWorldPoint(new Vector3(x, y, z));
    }

    Vector3 CalculateBezierCurve(Vector3 start, Vector3 end, float t)
    {
        Vector3 controlPoint = (start + end) * 0.5f + Vector3.up * flyCurveHeight;

        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * start;
        point += 2f * u * t * controlPoint;
        point += tt * end;

        return point;
    }
}