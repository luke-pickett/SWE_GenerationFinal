using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IEntity
{
    public Vector3Int CurrentTile { get; set; }
    public Directions.Direction FacingDirection { get; set; } = Directions.Direction.Down;
    
    [SerializeField] private int maxHealth = 20;
    private int health;
    private int attackDamage = 5;

    private void Awake()
    {
        health = maxHealth;
    }

    private void Start()
    {
        Debug.Log($"Player spawned with {health}/{maxHealth} HP");
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"Player takes {amount} damage! Health: {health}/{maxHealth}");
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        Debug.Log($"Player healed {amount}. Health: {health}/{maxHealth}");
    }   

    public void OnDeath()
    {
        Debug.Log("Player died! Game Over!");
        GridManager.Instance.RemoveEntity(CurrentTile);
        
        if (RoundManager.Instance != null && GameOverManager.Instance != null)
        {
            int finalScore = RoundManager.Instance.GetCurrentScore();
            GameOverManager.Instance.TriggerGameOver(finalScore);
        }
        
        gameObject.SetActive(false);
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void MoveToTile(Vector3Int newTile)
    {
        CurrentTile = newTile;
        transform.position = GridManager.Instance.GetWorldPosition(newTile);
    }

    private void OnMoveUp()
    {
        if (!GameLoop.Instance.IsPlayerTurn())
        {
            return;
        }

        FacingDirection = Directions.Direction.Up;
        GridManager.Instance.MoveEntity(gameObject, Directions.Direction.Up);
        GameLoop.Instance.OnPlayerAction();
    }

    private void OnMoveDown()
    {
        if (!GameLoop.Instance.IsPlayerTurn())
        {
            return;
        }

        FacingDirection = Directions.Direction.Down;
        GridManager.Instance.MoveEntity(gameObject, Directions.Direction.Down);
        GameLoop.Instance.OnPlayerAction();
    }

    private void OnMoveLeft()
    {
        if (!GameLoop.Instance.IsPlayerTurn())
        {
            return;
        }

        FacingDirection = Directions.Direction.Left;
        GridManager.Instance.MoveEntity(gameObject, Directions.Direction.Left);
        GameLoop.Instance.OnPlayerAction();
    }

    private void OnMoveRight()
    {
        if (!GameLoop.Instance.IsPlayerTurn())
        {
            return;
        }

        FacingDirection = Directions.Direction.Right;
        GridManager.Instance.MoveEntity(gameObject, Directions.Direction.Right);
        GameLoop.Instance.OnPlayerAction();
    }

    private void OnAttack()
    {
        if (!GameLoop.Instance.IsPlayerTurn())
        {
            return;
        }

        Vector3Int targetPosition = CurrentTile + Directions.GetDirectionVector(FacingDirection);
        
        if (GridManager.Instance.HasEntity(targetPosition))
        {
            IEntity targetEntity = GridManager.Instance.GetEntity(targetPosition);
            IDamageable damageable = (targetEntity as MonoBehaviour)?.GetComponent<IDamageable>();
            
            if (damageable != null)
            {
                Debug.Log($"Player attacks enemy at {targetPosition} for {attackDamage} damage!");
                damageable.TakeDamage(attackDamage);
            }
        }
        else
        {
            Debug.Log("Player attacks but nothing is there!");
        }

        GameLoop.Instance.OnPlayerAction();
    }
}
