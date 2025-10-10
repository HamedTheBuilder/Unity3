using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Transition Settings")]
    public float fadeOutDuration = 2f;
    public float fadeInDuration = 2f;
    public float blackScreenDuration = 1f;

    private Canvas fadeCanvas;
    private RawImage fadeImage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateFadeOverlay();
            Debug.Log("✅ TransitionManager جاهز");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateFadeOverlay()
    {
        // إنشاء كانفس يغطي كل الشاشة
        GameObject canvasObj = new GameObject("FadeCanvas");
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;

        // إنشاء الفيد image
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        fadeImage = imageObj.AddComponent<RawImage>();
        fadeImage.color = new Color(0, 0, 0, 0);

        // جعله يغطي كل الشاشة
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeImage.gameObject.SetActive(false);
        DontDestroyOnLoad(canvasObj);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        Debug.Log("🎬 بدء الانتقال مع فيد أسود...");

        // 1. فيد إلى الأسود
        yield return StartCoroutine(FadeOut());

        // 2. انتظار
        yield return new WaitForSeconds(blackScreenDuration);

        // 3. تحميل السين الجديد
        SceneManager.LoadScene(sceneName);

        // 4. فيد من الأسود إلى الشفاف
        yield return StartCoroutine(FadeIn());

        Debug.Log("✅ اكتمل الانتقال");
    }

    IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);

        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeOutDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = Color.black;
        Debug.Log("⬛ الشاشة سوداء بالكامل");
    }

    IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        fadeImage.color = Color.black;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeInDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false);
        Debug.Log("⬜ الشاشة ظهرت");
    }
}