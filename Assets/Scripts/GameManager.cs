using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float maxGameTime = 45f;
    public int scorePerAsteroid = 10;
    public int scorePerPowerUp = 25;

    [Header("UI References")]
    public Text countdownText;
    public Text scoreText;

    [Header("Scene Settings")]
    public string resultsSceneName = "ResultsScene";

    [Header("Sound Settings")]
    public AudioClip gameOverSound;
    public float soundVolume = 1f;

    [Header("Camera Fade Settings")]
    public float cameraFadeDuration = 2f;

    private int currentScore = 0;
    private float gameTime = 0f;
    private bool isGameOver = false;
    private bool isGameRunning = true;
    private bool soundPlayed = false;

    private AsteroidSpawner asteroidSpawner;
    private SpaceshipMovement spaceshipMovement;
    private AudioSource audioSource;
    private List<Camera> allCameras = new List<Camera>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FindGameComponents();
        InitializeCountdown();
        InitializeScoreText();
        FindAllCameras();
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

    void Update()
    {
        if (!isGameRunning || isGameOver) return;

        gameTime += Time.deltaTime;
        UpdateCountdown();

        if (gameTime >= maxGameTime)
        {
            GameOver("Time's Up!");
        }
    }

    void InitializeCountdown()
    {
        if (countdownText != null)
        {
            countdownText.text = Mathf.CeilToInt(maxGameTime).ToString();
        }
    }

    void InitializeScoreText()
    {
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    void UpdateCountdown()
    {
        if (countdownText != null)
        {
            float timeLeft = maxGameTime - gameTime;
            countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
        }
    }

    void FindGameComponents()
    {
        asteroidSpawner = FindAnyObjectByType<AsteroidSpawner>();
        spaceshipMovement = FindAnyObjectByType<SpaceshipMovement>();
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        UpdateScoreText();
    }

    public void AsteroidDestroyed()
    {
        AddScore(scorePerAsteroid);
    }

    public void PowerUpCollected()
    {
        AddScore(scorePerPowerUp);
    }

    public void PlayerHit()
    {
        if (!isGameOver)
        {
            GameOver("Destroyed by Asteroids!");
        }
    }

    void GameOver(string reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        isGameRunning = false;

        if (countdownText != null)
        {
            countdownText.text = "0";
        }

        PlayGameOverSound();
        StopAllGameSystems();
        StartCoroutine(TransitionToResults());
    }

    void PlayGameOverSound()
    {
        if (gameOverSound != null && audioSource != null && !soundPlayed)
        {
            audioSource.PlayOneShot(gameOverSound, soundVolume);
            soundPlayed = true;
        }
    }

    IEnumerator TransitionToResults()
    {
        // استخدام الفيد الجديد للخلفية
        yield return StartCoroutine(FadeBackgroundToBlack());

        yield return new WaitForSeconds(0.5f);

        string targetScene = resultsSceneName;

        if (IsSceneInBuildSettings(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    IEnumerator FadeBackgroundToBlack()
    {
        // حفظ الإعدادات الأصلية للكاميرات
        Dictionary<Camera, Color> originalColors = new Dictionary<Camera, Color>();
        Dictionary<Camera, CameraClearFlags> originalFlags = new Dictionary<Camera, CameraClearFlags>();

        foreach (Camera cam in allCameras)
        {
            if (cam != null)
            {
                originalColors[cam] = cam.backgroundColor;
                originalFlags[cam] = cam.clearFlags;
                cam.clearFlags = CameraClearFlags.SolidColor;
            }
        }

        // الفيد التدريجي للخلفية من اللون الأصلي إلى الأسود
        float timer = 0f;
        while (timer < cameraFadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / cameraFadeDuration;

            foreach (Camera cam in allCameras)
            {
                if (cam != null)
                {
                    // تحويل خلفية الكاميرا إلى الأسود تدريجياً
                    cam.backgroundColor = Color.Lerp(originalColors[cam], Color.black, progress);
                }
            }

            yield return null;
        }

        // التأكد من أن الخلفية سوداء بالكامل
        foreach (Camera cam in allCameras)
        {
            if (cam != null)
            {
                cam.backgroundColor = Color.black;
            }
        }
    }

    void FindAllCameras()
    {
        allCameras.Clear();
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        allCameras.AddRange(cameras);
    }

    bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string availableSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (availableSceneName == sceneName) return true;
        }
        return false;
    }

    void StopAllGameSystems()
    {
        if (asteroidSpawner != null)
            asteroidSpawner.StopSpawning();

        if (spaceshipMovement != null)
            spaceshipMovement.enabled = false;

        SimpleLaserGun laserGun = FindAnyObjectByType<SimpleLaserGun>();
        if (laserGun != null)
            laserGun.enabled = false;

        PowerUpSystem powerUpSystem = FindAnyObjectByType<PowerUpSystem>();
        if (powerUpSystem != null)
            powerUpSystem.DeactivateAllPowerUps();
    }

    public int GetFinalScore()
    {
        return currentScore;
    }

    public void ResetGame()
    {
        currentScore = 0;
        gameTime = 0f;
        isGameOver = false;
        isGameRunning = true;
        soundPlayed = false;

        if (countdownText != null)
        {
            countdownText.text = Mathf.CeilToInt(maxGameTime).ToString();
        }

        UpdateScoreText();
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    public bool IsGameRunning
    {
        get { return isGameRunning; }
    }
}