using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static Action GameOverManagerInstanced;
    public static Action GameOver;
    public static GameOverManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameData gameData;
    [SerializeField] private GameObject gameOverPanel;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameOverManagerInstanced?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void TriggerGameOver(int finalScore)
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log($"Game Over! Final Score: {finalScore}");

        if (gameData != null)
        {
            gameData.HighScore = finalScore;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        GameOver?.Invoke();
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public int GetHighScore()
    {
        return gameData != null ? gameData.HighScore : 0;
    }
}
