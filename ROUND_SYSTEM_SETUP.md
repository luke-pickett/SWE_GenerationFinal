# Round-Based System Setup Instructions

## Overview
A complete round-based wave system has been implemented with the following features:
- Increasing enemy spawns per round
- Player healing between rounds
- Score tracking for enemy kills
- UI display for round, score, health, and enemies remaining

## New Files Created

### 1. `Assets/Systems/RoundManager.cs`
Manages the round-based gameplay system.

**Serialized Fields (Set in Inspector):**
- `baseEnemiesPerRound` (default: 2) - Starting number of enemies
- `enemiesIncreaseRate` (default: 1.5) - Multiplier for enemy count each round
- `healAmountPerRound` (default: 5) - HP restored to player between rounds
- `scorePerKill` (default: 10) - Points awarded per enemy killed
- `spiderPrefab` - Reference to the Spider enemy prefab
- `spawnDelay` (default: 0.5s) - Time between enemy spawns

### 2. `Assets/UI/UIManager.cs`
Manages UI display for game information.

**Serialized Fields (Set in Inspector):**
- `roundText` - TextMeshProUGUI for displaying current round
- `scoreText` - TextMeshProUGUI for displaying score
- `healthText` - TextMeshProUGUI for displaying player health
- `enemiesText` - TextMeshProUGUI for displaying remaining enemies

## Setup Steps

### 1. Add RoundManager to Scene
1. Create an empty GameObject in your scene
2. Name it "RoundManager"
3. Add the `RoundManager` component
4. Assign the Spider prefab to the `spiderPrefab` field
5. Adjust round settings as desired

### 2. Create UI Canvas
1. Create a Canvas in your scene (UI > Canvas)
2. Add TextMeshProUGUI elements for:
   - Round display (top-left)
   - Score display (top-right)
   - Health display (bottom-left)
   - Enemies remaining (top-center)

### 3. Add UIManager to Scene
1. Create an empty GameObject in your scene
2. Name it "UIManager"
3. Add the `UIManager` component
4. Assign the TextMeshProUGUI components to the respective fields

### 4. Update GameLoop GameObject
1. Remove the `spiderEnemyPrefab` field assignment (no longer needed)

## How It Works

### Round Flow
1. **Round Start**: RoundManager spawns enemies based on formula: `baseEnemies * (increaseRate ^ (round - 1))`
2. **Combat**: Player fights enemies in turn-based combat
3. **Enemy Death**: Score increases, enemy count decreases
4. **Round End**: When all enemies are defeated:
   - Player heals for `healAmountPerRound`
   - 3-second delay before next round
   - Next round starts with more enemies

### Enemy Scaling
- Round 1: 2 enemies
- Round 2: 3 enemies
- Round 3: 5 enemies
- Round 4: 7 enemies
- Round 5: 11 enemies
- etc.

### Score System
- Each enemy killed awards 10 points (configurable)
- Score persists across rounds
- Displayed in UI

### Healing System
- Player heals 5 HP at the end of each round (configurable)
- Health cannot exceed maximum (20 HP default)
- Healing happens automatically

## Events Available

The system provides these events for extending functionality:

```csharp
// RoundManager events
RoundManager.RoundStarted(int round)
RoundManager.RoundEnded(int round)
RoundManager.ScoreChanged(int score)
```

## Code Changes Made

### Player.cs
- Added `maxHealth` field
- Added `GetHealth()` and `GetMaxHealth()` methods
- Improved health display in debug logs
- Health capped at maximum when healing

### Spider.cs
- Calls `RoundManager.Instance.OnEnemyKilled()` on death
- Notifies round system for score tracking

### GameLoop.cs
- Removed spider spawning (now handled by RoundManager)
- Cleaned up unused spider prefab reference

## Testing

1. Play the game
2. Kill all enemies in round 1
3. Observe:
   - Player heals
   - New round starts after delay
   - More enemies spawn
   - Score increases per kill
   - UI updates correctly

## Customization

### Adjust Difficulty
- Increase `baseEnemiesPerRound` for harder start
- Increase `enemiesIncreaseRate` for faster scaling
- Decrease `healAmountPerRound` for less sustain
- Adjust Spider health/damage in Spider.cs

### Change Scoring
- Modify `scorePerKill` in RoundManager
- Add combo bonuses by tracking consecutive kills
- Add time bonuses for fast round completion

### Add Features
- Boss rounds every X rounds
- Special enemy types with different spawn rates
- Power-ups spawning between rounds
- Difficulty modifiers (easy/normal/hard)
