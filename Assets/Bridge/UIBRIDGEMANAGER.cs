using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
        // › Õ Ê≈€·«ﬁ ﬁ«∆„… «·≈Ìﬁ«› »“— ESC
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
        // “— «·„Ê«’·…
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        // “— ≈⁄«œ… «··⁄»…
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        // “— «·⁄Êœ… ··„«Ì‰ „‰ÌÊ
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // “— «·Œ—ÊÃ „‰ «··⁄»…
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void ResumeGame()
    {
        // ≈Œ›«¡ ‘«‘… «·≈Ìﬁ«›
        HidePauseMenu();

        // ≈⁄«œ… «··⁄»…
        Time.timeScale = 1f;

        //  ‘€Ì· ’Ê  «·“— ≈–« ﬂ«‰ AudioManager „ÊÃÊœ
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
            Time.timeScale = 0f; // ≈Ìﬁ«› «··⁄»…

            //  ‘€Ì· ’Ê  › Õ «·ﬁ«∆„…
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
            Time.timeScale = 1f; // «” ∆‰«› «··⁄»…
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

        //  ‘€Ì· ’Ê  Ã„⁄ «·⁄„·…
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

            //  ‘€Ì· ’Ê  ‰Â«Ì… «··⁄»…
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

        //  ‘€Ì· ’Ê  «·“—
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // ≈⁄«œ…  Õ„Ì· «·”Ì‰ «·Õ«·Ì
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        //  ‘€Ì· ’Ê  «·“—
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // «·«‰ ﬁ«· ≈·Ï ”Ì‰ «·„«Ì‰ „‰ÌÊ
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            // ≈–« ·„ Ì „  ⁄ÌÌ‰ «”„ «·”Ì‰° Õ«Ê·  Õ„Ì· «·”Ì‰ «·√Ê·
            SceneManager.LoadScene(0);
        }
    }

    public void QuitGame()
    {
        //  ‘€Ì· ’Ê  «·“—
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        //  √ŒÌ— «·Œ—ÊÃ ﬁ·Ì·« ·”„«⁄ «·’Ê 
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