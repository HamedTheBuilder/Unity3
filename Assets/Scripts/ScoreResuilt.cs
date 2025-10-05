using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreResultManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public RawImage[] resultImages; // 5 صور للنتائج
    public Text countingText;
    public Canvas resultsCanvas;
    public Button menuButton;
    public Button nextLevelButton;

    [Header("Score Settings")]
    public int[] scoreThresholds = { 200, 400, 600, 800, 801 }; // عتبات النقاط الجديدة
    public float countingDuration = 3f; // مدة عد النقاط

    [Header("Animation Settings")]
    public float blackScreenDuration = 2f; // مدة الشاشة السوداء
    public float fadeDuration = 1f; // مدة الفيد التدريجي

    [Header("Sound Settings")]
    public AudioClip scoreCountingSound; // صوت أثناء عد النقاط
    public AudioClip resultAppearSound; // صوت عند ظهور النتيجة
    public float soundVolume = 1f;

    private int finalScore;
    private int currentDisplayedScore = 0;
    private Image blackScreen;
    private AudioSource audioSource;
    private bool isCounting = false;

    void Start()
    {
        // إنشاء AudioSource
        CreateAudioSource();

        // إنشاء الشاشة السوداء
        CreateBlackScreen();

        // إخفاء الـ UI في البداية
        if (resultsCanvas != null)
            resultsCanvas.gameObject.SetActive(false);

        // إخفاء الأزرار
        if (menuButton != null)
            menuButton.gameObject.SetActive(false);
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);

        // جلب النقاط النهائية من الـ GameManager
        finalScore = GameManager.Instance != null ? GameManager.Instance.GetFinalScore() : 0;

        // إخفاء كل الصور في البداية
        HideAllImages();

        // إعداد الأزرار
        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMainMenu);
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(GoToNextLevel);

        // بدء تسلسل العرض
        StartCoroutine(DisplaySequence());
    }

    void CreateAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = soundVolume;
            audioSource.playOnAwake = false;
        }
    }

    void CreateBlackScreen()
    {
        // إنشاء كائن للشاشة السوداء
        GameObject blackScreenObject = new GameObject("BlackScreen");
        Canvas canvas = blackScreenObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // فوق كل شيء

        blackScreen = blackScreenObject.AddComponent<Image>();
        blackScreen.color = Color.black; // سوداء من البداية

        // جعلها تغطي كل الشاشة
        RectTransform rect = blackScreen.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        DontDestroyOnLoad(blackScreenObject);
    }

    IEnumerator DisplaySequence()
    {
        // 1. الشاشة سوداء في البداية لمدة 2 ثانية
        yield return new WaitForSeconds(blackScreenDuration);

        // 2. إخفاء الشاشة السوداء تدريجياً
        yield return StartCoroutine(FadeOutBlackScreen());

        // 3. عرض شاشة النتائج
        ShowResultsCanvas();

        // 4. عد النقاط من 0 إلى النقاط النهائية
        yield return StartCoroutine(CountScoreRoutine());

        // 5. عرض الصورة المناسبة بعد انتهاء العد
        ShowResultImage();

        // 6. تفعيل الأزرار بعد 2 ثانية
        yield return new WaitForSeconds(2f);
        EnableButtons();
    }

    IEnumerator FadeOutBlackScreen()
    {
        if (blackScreen == null) yield break;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // إخفاء الشاشة السوداء بالكامل
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.gameObject.SetActive(false);
    }

    void ShowResultsCanvas()
    {
        if (resultsCanvas != null)
        {
            resultsCanvas.gameObject.SetActive(true);
        }
    }

    IEnumerator CountScoreRoutine()
    {
        isCounting = true;
        float timer = 0f;

        // بدء صوت العد
        StartCountingSound();

        while (timer < countingDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / countingDuration;

            // حساب النقاط المعروضة
            currentDisplayedScore = Mathf.RoundToInt(finalScore * progress);

            // تحديث النص
            UpdateScoreText();

            yield return null;
        }

        // التأكد من الوصول للرقم النهائي
        currentDisplayedScore = finalScore;
        UpdateScoreText();

        // إيقاف صوت العد
        isCounting = false;
        StopCountingSound();
    }

    void StartCountingSound()
    {
        if (scoreCountingSound != null && audioSource != null)
        {
            // تشغيل الصوت بشكل متكرر أثناء العد
            InvokeRepeating("PlayCountingSound", 0f, 0.1f);
        }
    }

    void StopCountingSound()
    {
        CancelInvoke("PlayCountingSound");
    }

    void PlayCountingSound()
    {
        if (isCounting && scoreCountingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(scoreCountingSound, soundVolume * 0.3f);
        }
    }

    void UpdateScoreText()
    {
        if (countingText != null)
        {
            countingText.text = currentDisplayedScore.ToString(); // رقم فقط بدون كلمة
        }
    }

    void ShowResultImage()
    {
        // إخفاء كل الصور أولاً
        HideAllImages();

        // تحديد الصورة المناسبة بناءً على النقاط
        int imageIndex = GetResultImageIndex();

        if (imageIndex >= 0 && imageIndex < resultImages.Length && resultImages[imageIndex] != null)
        {
            resultImages[imageIndex].gameObject.SetActive(true);

            // تشغيل صوت ظهور النتيجة
            PlayResultAppearSound();

            // تأثير ظهور الصورة
            StartCoroutine(ImageAppearAnimation(resultImages[imageIndex]));
        }
    }

    void PlayResultAppearSound()
    {
        if (resultAppearSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(resultAppearSound, soundVolume);
        }
    }

    int GetResultImageIndex()
    {
        // تحديد الصورة حسب النقاط:
        // 0: 0-200
        // 1: 201-400  
        // 2: 401-600
        // 3: 601-800
        // 4: 801 فأكثر

        if (finalScore <= 200)
            return 0;
        else if (finalScore <= 400)
            return 1;
        else if (finalScore <= 600)
            return 2;
        else if (finalScore <= 800)
            return 3;
        else
            return 4; // 801 فأكثر
    }

    void HideAllImages()
    {
        foreach (RawImage image in resultImages)
        {
            if (image != null)
                image.gameObject.SetActive(false);
        }
    }

    IEnumerator ImageAppearAnimation(RawImage image)
    {
        if (image == null) yield break;

        // تأثير ظهور تدريجي
        image.transform.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            image.transform.localScale = Vector3.one * progress;
            yield return null;
        }

        image.transform.localScale = Vector3.one;
    }

    void EnableButtons()
    {
        // إظهار الأزرار
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(true);
            StartCoroutine(ButtonAppearAnimation(menuButton));
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
            StartCoroutine(ButtonAppearAnimation(nextLevelButton));
        }
    }

    IEnumerator ButtonAppearAnimation(Button button)
    {
        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        float timer = 0f;
        float duration = 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / duration;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    // دوال للأزرار
    public void GoToMainMenu()
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionToScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void GoToNextLevel()
    {
        string nextScene = "NextLevel"; // غير هذا لاسم المرحلة التالية

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionToScene(nextScene);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    public void RestartGame()
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionToScene("GameScene");
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    void OnDestroy()
    {
        // تنظيف الشاشة السوداء إذا دمر السكربت
        if (blackScreen != null && blackScreen.gameObject != null)
        {
            Destroy(blackScreen.gameObject);
        }
    }
}