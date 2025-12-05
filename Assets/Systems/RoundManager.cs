using System;
using System.Collections;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static Action RoundManagerInstanced;
    public static Action<int> RoundStarted;
    public static Action<int> RoundEnded;
    public static Action<int> ScoreChanged;

    public static RoundManager Instance { get; private set; }

    [Header("Round Settings")]
    [SerializeField] private int baseEnemiesPerRound = 2;
    [SerializeField] private float enemiesIncreaseRate = 1.5f;
    [SerializeField] private int healAmountPerRound = 5;
    [SerializeField] private int scorePerKill = 10;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private float spawnDelay = 0.5f;

    private int currentRound = 0;
    private int currentScore = 0;
    private int enemiesAliveThisRound = 0;
    private int enemiesToSpawnThisRound = 0;
    private bool roundInProgress = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            RoundManagerInstanced?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        MapGenerator.MapGenerated += OnMapGenerated;
    }

    private void OnDisable()
    {
        MapGenerator.MapGenerated -= OnMapGenerated;
    }

    private void OnMapGenerated()
    {
        StartCoroutine(StartNextRoundAfterDelay(2f));
    }

    private IEnumerator StartNextRoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextRound();
    }

    public void StartNextRound()
    {
        if (roundInProgress)
        {
            return;
        }

        currentRound++;
        roundInProgress = true;
        enemiesToSpawnThisRound = Mathf.CeilToInt(baseEnemiesPerRound * Mathf.Pow(enemiesIncreaseRate, currentRound - 1));
        enemiesAliveThisRound = enemiesToSpawnThisRound;

        Debug.Log($"Round {currentRound} started! Spawning {enemiesToSpawnThisRound} enemies.");
        RoundStarted?.Invoke(currentRound);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawnThisRound; i++)
        {
            Vector3Int spawnPosition = GridManager.Instance.GetRandomPathableTile();
            
            while (GridManager.Instance.HasEntity(spawnPosition))
            {
                spawnPosition = GridManager.Instance.GetRandomPathableTile();
            }

            SpawnEnemy(spawnPosition);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy(Vector3Int gridSpawnPosition)
    {
        Vector3 worldSpacePosition = GridManager.Instance.GetWorldPosition(gridSpawnPosition);
        GameObject enemy = Instantiate(spiderPrefab, worldSpacePosition, Quaternion.identity);
        IEntity entityComponent = enemy.GetComponent<IEntity>();
        entityComponent.CurrentTile = gridSpawnPosition;
        GridManager.Instance.SetEntity(gridSpawnPosition, entityComponent);
    }

    public void OnEnemyKilled()
    {
        enemiesAliveThisRound--;
        currentScore += scorePerKill;
        
        Debug.Log($"Enemy killed! Score: {currentScore}, Enemies remaining: {enemiesAliveThisRound}");
        ScoreChanged?.Invoke(currentScore);

        if (enemiesAliveThisRound <= 0 && roundInProgress)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        roundInProgress = false;
        Debug.Log($"Round {currentRound} complete! Score: {currentScore}");
        RoundEnded?.Invoke(currentRound);

        HealPlayer();
        StartCoroutine(StartNextRoundAfterDelay(3f));
    }

    private void HealPlayer()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.Heal(healAmountPerRound);
            Debug.Log($"Player healed for {healAmountPerRound} HP!");
        }
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetEnemiesRemaining()
    {
        return enemiesAliveThisRound;
    }
}
