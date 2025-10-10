using System.Collections;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int startingLives = 3;

    private int currentLives;
    private bool gamePaused = false;

    void Start()
    {
        currentLives = startingLives;

        // Initialize UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHearts(currentLives);
            UIManager.Instance.UpdateCoinDisplay(0);
        }
    }

    void Update()
    {
        HandlePauseInput();
    }

    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;

        if (UIManager.Instance != null)
        {
            if (gamePaused)
                UIManager.Instance.ShowPauseMenu();
            else
                UIManager.Instance.HidePauseMenu();
        }
    }

    public void PlayerDied()
    {
        currentLives--;

        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHearts(currentLives);
        }

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Show game over screen
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }

    public void AddCoins(int amount)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.AddCoins(amount);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}