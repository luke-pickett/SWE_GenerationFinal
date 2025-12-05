using UnityEngine;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static Action UIManagerInstanced;
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject inGameUIPanel;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;

    private Player player;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            UIManagerInstanced?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
        RoundManager.ScoreChanged += OnScoreChanged;
        MapGenerator.MapGenerated += OnMapGenerated;
        GameOverManager.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted -= OnRoundStarted;
        RoundManager.ScoreChanged -= OnScoreChanged;
        MapGenerator.MapGenerated -= OnMapGenerated;
        GameOverManager.GameOver -= OnGameOver;
    }

    private void OnMapGenerated()
    {
        player = FindFirstObjectByType<Player>();
        isInitialized = true;
        
        if (inGameUIPanel != null)
        {
            inGameUIPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (isInitialized)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
            if (player == null) return;
        }

        if (RoundManager.Instance != null)
        {
            if (roundText != null)
            {
                roundText.text = $"Round: {RoundManager.Instance.GetCurrentRound()}";
            }

            if (scoreText != null)
            {
                scoreText.text = $"Score: {RoundManager.Instance.GetCurrentScore()}";
            }

            if (enemiesText != null)
            {
                enemiesText.text = $"Enemies: {RoundManager.Instance.GetEnemiesRemaining()}";
            }
        }

        if (GameOverManager.Instance != null && highScoreText != null)
        {
            highScoreText.text = $"High Score: {GameOverManager.Instance.GetHighScore()}";
        }

        if (healthText != null)
        {
            int currentHealth = player.GetHealth();
            int maxHealth = player.GetMaxHealth();
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }

    private void OnRoundStarted(int round)
    {
        Debug.Log($"UI: Round {round} started");
    }

    private void OnScoreChanged(int score)
    {
        Debug.Log($"UI: Score updated to {score}");
    }

    private void OnGameOver()
    {
        if (inGameUIPanel != null)
        {
            inGameUIPanel.SetActive(false);
        }

        if (RoundManager.Instance != null && GameOverManager.Instance != null)
        {
            int finalScore = RoundManager.Instance.GetCurrentScore();
            int highScore = GameOverManager.Instance.GetHighScore();

            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = $"Final Score: {finalScore}";
            }

            if (gameOverHighScoreText != null)
            {
                gameOverHighScoreText.text = $"High Score: {highScore}";
            }
        }
    }
}
