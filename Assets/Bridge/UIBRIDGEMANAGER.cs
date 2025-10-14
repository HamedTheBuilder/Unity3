using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements - Text Mesh Pro")]
    public TextMeshProUGUI coinText;
    public GameObject[] healthHearts;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public GameObject pausePanel;

    [Header("Pause Menu Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private int currentCoins = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize UI
        UpdateCoinDisplay(0);
        UpdateHearts(3);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Setup button listeners
        SetupButtonListeners();
    }

    void Update()
    {
        // فتح وإغلاق قائمة الإيقاف بزر ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel != null && pausePanel.activeInHierarchy)
            {
                ResumeGame();
            }
            else
            {
                ShowPauseMenu();
            }
        }
    }

    void SetupButtonListeners()
    {
        // زر المواصلة
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        // زر إعادة اللعبة
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        // زر العودة للماين منيو
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // زر الخروج من اللعبة
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void ResumeGame()
    {
        // إخفاء شاشة الإيقاف
        HidePauseMenu();

        // إعادة اللعبة
        Time.timeScale = 1f;

        // تشغيل صوت الزر إذا كان AudioManager موجود
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f; // إيقاف اللعبة

            // تشغيل صوت فتح القائمة
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        }
    }

    public void HidePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f; // استئناف اللعبة
        }
    }

    public void UpdateCoinDisplay(int coins)
    {
        currentCoins = coins;
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinDisplay(currentCoins);

        // تشغيل صوت جمع العملة
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinCollect();
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        if (healthHearts == null) return;

        for (int i = 0; i < healthHearts.Length; i++)
        {
            if (healthHearts[i] != null)
                healthHearts[i].SetActive(i < currentHealth);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.text = "Game Over!\nCoins Collected: " + currentCoins;

            // تشغيل صوت نهاية اللعبة
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOver();
            }
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // تشغيل صوت الزر
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // إعادة تحميل السين الحالي
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // تشغيل صوت الزر
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // الانتقال إلى سين الماين منيو
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            // إذا لم يتم تعيين اسم السين، حاول تحميل السين الأول
            SceneManager.LoadScene(0);
        }
    }

    public void QuitGame()
    {
        // تشغيل صوت الزر
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // تأخير الخروج قليلاً لسماع الصوت
        Invoke("QuitApplication", 0.3f);
    }

    void QuitApplication()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}