using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spider : MonoBehaviour, IDamageable, IEntity
{
    public Vector3Int CurrentTile { get; set; }
    public Directions.Direction FacingDirection { get; set; } = Directions.Direction.Down;
    private int health = 10;
    private int attackDamage = 2;

    private void Start()
    {
        GameLoop.Instance.RegisterEnemy(this);
        GameLoop.EnemyTurnStart += OnEnemyTurnStart;
    }

    private void OnDestroy()
    {
        if (GameLoop.Instance != null)
        {
            GameLoop.Instance.UnregisterEnemy(this);
        }
        GameLoop.EnemyTurnStart -= OnEnemyTurnStart;
    }

    private void OnEnemyTurnStart()
    {
        TakeTurn();
    }

    private void TakeTurn()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            return;
        }

        Vector3Int playerPosition = player.CurrentTile;
        int distanceToPlayer = Mathf.Abs(CurrentTile.x - playerPosition.x) + Mathf.Abs(CurrentTile.y - playerPosition.y);

        if (distanceToPlayer == 1)
        {
            AttackPlayer(player);
        }
        else
        {
            MoveTowardsPlayer(playerPosition);
        }
    }

    private void MoveTowardsPlayer(Vector3Int playerPosition)
    {
        List<Vector3Int> path = PathfindingService.Instance.GetPath(CurrentTile, playerPosition);

        if (path == null)
        {
            Debug.LogWarning("Spider: No path to player found!");
            return;
        }

        if (path.Count > 1)
        {
            Vector3Int nextPosition = path[1];

            if (GridManager.Instance.IsWalkable(nextPosition) && !GridManager.Instance.HasEntity(nextPosition))
            {
                Vector3Int oldPosition = CurrentTile;
                GridManager.Instance.RemoveEntity(CurrentTile);
                MoveToTile(nextPosition);
                GridManager.Instance.SetEntity(nextPosition, this);

                UpdateFacingDirection(oldPosition, nextPosition);
            }
        }
    }

    private void UpdateFacingDirection(Vector3Int oldPosition, Vector3Int newPosition)
    {
        Vector3Int delta = newPosition - oldPosition;
        if (delta.x > 0) FacingDirection = Directions.Direction.Right;
        else if (delta.x < 0) FacingDirection = Directions.Direction.Left;
        else if (delta.y > 0) FacingDirection = Directions.Direction.Up;
        else if (delta.y < 0) FacingDirection = Directions.Direction.Down;
    }

    private void AttackPlayer(Player player)
    {
        Debug.Log($"Spider attacks player for {attackDamage} damage!");
        player.TakeDamage(attackDamage);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"Spider takes {amount} damage! Health: {health}");
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    public void OnDeath()
    {
        Debug.Log("Spider defeated!");
        GridManager.Instance.RemoveEntity(CurrentTile);
        
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.OnEnemyKilled();
        }
        
        Destroy(gameObject);
    }

    public void MoveToTile(Vector3Int newTile)
    {
        CurrentTile = newTile;
        transform.position = GridManager.Instance.GetWorldPosition(newTile);
    }
}
