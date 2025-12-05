# System Architecture & Design

This document provides an in-depth explanation of the system architecture, design patterns, major classes, and design decisions for the game.

## Architecture Overview

### High-Level Architecture

The project follows a **hybrid MVC (Model-View-Controller)** architecture adapted for Unity's component-based system.
Some key features include:
- Clear boundaries between data, logic, and presentation
- Loose coupling via C# events
- Centralized access to game systems
- Polymorphic entity behavior

## Design Patterns

### 1. Singleton Pattern

**Purpose**: Ensure only one instance of critical managers exists and provide global access.

**Implementation**:
```csharp
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    
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
}
```

**Used In**:
- `GameLoop`
- `GridManager`
- `RoundManager`
- `PathfindingService`
- `MapGenerator`
- `UIManager`
- `GameOverManager`

**Reasoning**: Unity's scene-based architecture benefits from singleton access patterns for frequently referenced systems..

### 2. Observer Pattern (Event-Driven)

**Purpose**: To decouple components via event subscriptions instead of direct references.

**Implementation**:
```csharp
public class RoundManager : MonoBehaviour
{
    public static Action<int> RoundStarted;
    public static Action<int> ScoreChanged;
    
    public void StartNextRound()
    {
        currentRound++;
        RoundStarted?.Invoke(currentRound);
    }
}

public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        RoundManager.RoundStarted += OnRoundStarted;
    }
    
    private void OnRoundStarted(int round)
    {
        roundText.text = $"Round: {round}";
    }
}
```

**Event Examples**:

| Event Source | Event Name | Purpose |
|--------------|------------|---------|
| `GameLoop` | `PlayerTurnStart` | Notify UI/systems of player turn |
| `GameLoop` | `EnemyTurnStart` | Trigger enemy AI processing |
| `RoundManager` | `RoundStarted` | Update UI with new round |
| `RoundManager` | `ScoreChanged` | Refresh score display |
| `MapGenerator` | `MapGenerated` | Signal map ready for play |
| `GameOverManager` | `GameOver` | Show game over screen |

### 3. State Machine Pattern

**Purpose**: Manage turn-based game flow with clear states.

**Implementation** (in `GameLoop`):
```csharp
private enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    Processing
}

private TurnState _currentTurnState = TurnState.Processing;

public bool IsPlayerTurn()
{
    return _currentTurnState == TurnState.PlayerTurn;
}

private void StartPlayerTurn()
{
    _currentTurnState = TurnState.PlayerTurn;
    PlayerTurnStart?.Invoke();
}
```

**State Transitions**:
```
Processing → PlayerTurn → Processing → EnemyTurn → Processing → PlayerTurn...
```

**Reasoning**:
- Creates clear game flow logic
- Easy to debug (compared to using strings to keep track of state)

## Conclusion

This project demonstrates a solid foundation for a turn-based roguelike game using Unity. The architecture, while simple, effectively separates concerns and allows for future scalability.

If I were to continue developing this project, I would focus on more modularity, some systems are tightly coupled and while this works for a small project, it may become a burden later.