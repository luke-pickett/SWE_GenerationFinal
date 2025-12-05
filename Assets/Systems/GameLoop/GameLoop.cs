using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static Action GameStart;
    public static Action FireMapGeneration;
    public static Action PlayerTurnStart;
    public static Action PlayerTurnEnd;
    public static Action EnemyTurnStart;
    public static Action EnemyTurnEnd;

    public static GameLoop Instance { get; private set; }

    private bool _gridManagerInstanced = false;
    private bool _pathfindingServiceInstanced = false;
    private bool _mapGeneratorInstanced = false;
    private bool _uiManagerInstanced = false;
    private bool _roundManagerInstanced = false;

    private bool _mapGenerated = false;

    [SerializeField] private GameObject playerPrefab;

    private enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        Processing
    }

    private TurnState _currentTurnState = TurnState.Processing;
    private bool _playerActionTaken = false;
    private List<IEntity> _enemies = new List<IEntity>();

    private void Awake()
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

    private void OnEnable()
    {
        GridManager.GridManagerInstanced += OnGridManagerInstanced;
        PathfindingService.PathfindingServiceInstanced += OnPathfindingServiceInstanced;
        MapGenerator.MapGeneratorInstanced += OnMapGeneratorInstanced;
        UIManager.UIManagerInstanced += OnUIManagerInstanced;
        RoundManager.RoundManagerInstanced += OnRoundManagerInstanced;

        MapGenerator.MapGenerated += OnMapGenerated;
        
        if (GridManager.Instance != null) OnGridManagerInstanced();
        if (PathfindingService.Instance != null) OnPathfindingServiceInstanced();
        if (MapGenerator.Instance != null) OnMapGeneratorInstanced();
        if (UIManager.Instance != null) OnUIManagerInstanced();
        if (RoundManager.Instance != null) OnRoundManagerInstanced();
    }

    void OnGridManagerInstanced()
    {
        _gridManagerInstanced = true;
    }

    void OnPathfindingServiceInstanced()
    {
        _pathfindingServiceInstanced = true;
    }

    void OnMapGeneratorInstanced()
    {
        _mapGeneratorInstanced = true;
    }

    void OnUIManagerInstanced()
    {
        _uiManagerInstanced = true;
    }

    void OnRoundManagerInstanced()
    {
        _roundManagerInstanced = true;
    }

    void OnMapGenerated()
    {
        _mapGenerated = true;
    }

    private IEnumerator WaitForSingletons()
    {
        Debug.Log("Waiting for singletons to initialize...");
        Debug.Log($"GridManager: {_gridManagerInstanced}, PathfindingService: {_pathfindingServiceInstanced}, MapGenerator: {_mapGeneratorInstanced}, UIManager: {_uiManagerInstanced}, RoundManager: {_roundManagerInstanced}");
        
        while (!(_gridManagerInstanced && _pathfindingServiceInstanced && _mapGeneratorInstanced))
        {
            yield return null;
        }
        
        Debug.Log("Core singletons initialized!");
        
        float timeout = 2f;
        float elapsed = 0f;
        while (!(_uiManagerInstanced && _roundManagerInstanced) && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (!_uiManagerInstanced)
        {
            Debug.LogWarning("UIManager not found - UI will not display");
        }
        
        if (!_roundManagerInstanced)
        {
            Debug.LogWarning("RoundManager not found - rounds will not work");
        }
        
        Debug.Log("All singletons ready!");
    }

    private IEnumerator WaitForMapGeneration()
    {
        while (!_mapGenerated)
        {
            yield return null;
        }
    }

    private IEnumerator Start()
    {
        Debug.Log("GameLoop started");
        yield return StartCoroutine(WaitForSingletons());
        FireMapGeneration?.Invoke();
        yield return StartCoroutine(WaitForMapGeneration());
        Vector3Int playerSpawnPosition = GridManager.Instance.GetRandomPathableTile();
        SpawnEntity(playerPrefab, playerSpawnPosition);
        StartPlayerTurn();
    }

    void SpawnEntity(GameObject entityPrefab, Vector3Int gridSpawnPosition)
    {
        Vector3 worldSpacePosition = GridManager.Instance.GetWorldPosition(gridSpawnPosition);
        GameObject entity = Instantiate(entityPrefab, worldSpacePosition, Quaternion.identity);
        IEntity entityComponent = entity.GetComponent<IEntity>();
        entityComponent.CurrentTile = gridSpawnPosition;
        GridManager.Instance.SetEntity(gridSpawnPosition, entityComponent);
    }

    public void RegisterEnemy(IEntity enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(IEntity enemy)
    {
        _enemies.Remove(enemy);
    }

    public bool IsPlayerTurn()
    {
        return _currentTurnState == TurnState.PlayerTurn;
    }

    public void OnPlayerAction()
    {
        if (_currentTurnState == TurnState.PlayerTurn && !_playerActionTaken)
        {
            _playerActionTaken = true;
            EndPlayerTurn();
        }
    }

    private void StartPlayerTurn()
    {
        _currentTurnState = TurnState.PlayerTurn;
        _playerActionTaken = false;
        PlayerTurnStart?.Invoke();
        Debug.Log("Player Turn Started");
    }

    private void EndPlayerTurn()
    {
        _currentTurnState = TurnState.Processing;
        PlayerTurnEnd?.Invoke();
        Debug.Log("Player Turn Ended");
        StartCoroutine(ProcessEnemyTurns());
    }

    private IEnumerator ProcessEnemyTurns()
    {
        _currentTurnState = TurnState.EnemyTurn;
        EnemyTurnStart?.Invoke();
        Debug.Log("Enemy Turn Started");

        List<IEntity> enemiesCopy = new List<IEntity>(_enemies);

        foreach (IEntity enemy in enemiesCopy)
        {
            if (enemy != null && _enemies.Contains(enemy))
            {
                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return new WaitForSeconds(0.3f);
        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        _currentTurnState = TurnState.Processing;
        EnemyTurnEnd?.Invoke();
        Debug.Log("Enemy Turn Ended");
        StartPlayerTurn();
    }
}
