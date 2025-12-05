# High Score and Game Over System Setup

## Overview
A complete high score system with ScriptableObject persistence and game over UI has been implemented.

## New Files Created

### 1. `Assets/Data/GameData.cs`
ScriptableObject for persistent high score storage.

**Features:**
- Stores high score
- Only updates if new score is higher
- Can be reset
- Persists between play sessions in the editor

### 2. `Assets/Systems/GameOverManager.cs`
Manages game over state and restart functionality.

**Serialized Fields:**
- `gameData` - Reference to GameData ScriptableObject
- `gameOverPanel` - Reference to Game Over UI panel

**Public Methods:**
- `TriggerGameOver(int finalScore)` - Called when player dies
- `RestartGame()` - Reloads the current scene
- `QuitGame()` - Quits the application
- `GetHighScore()` - Returns current high score

## Setup Steps

### 1. Create GameData ScriptableObject
1. In Project window, right-click in `Assets/Data/` folder
2. Create > Game > Game Data
3. Name it "GameData"
4. This will store your high score

### 2. Add GameOverManager to Scene
1. Create an empty GameObject in your scene
2. Name it "GameOverManager"
3. Add the `GameOverManager` component
4. Assign the GameData ScriptableObject to the `gameData` field

### 3. Create Game Over UI Panel
Create a UI panel with the following elements:

```
Canvas
??? GameOverPanel
    ??? Background (Image - semi-transparent black)
    ??? GameOverText (TextMeshPro - "GAME OVER")
    ??? FinalScoreText (TextMeshPro - "Final Score: 0")
    ??? HighScoreText (TextMeshPro - "High Score: 0")
    ??? RestartButton (Button with TextMeshPro - "Restart")
    ??? QuitButton (Button with TextMeshPro - "Quit")
```

### 4. Configure GameOverManager
1. Assign the GameOverPanel to the `gameOverPanel` field
2. The panel will be hidden on start and shown on game over

### 5. Configure Restart Button
1. Select the Restart Button
2. In Button component, add OnClick event
3. Drag GameOverManager GameObject to the object field
4. Select `GameOverManager.RestartGame` from the function dropdown

### 6. Configure Quit Button (Optional)
1. Select the Quit Button
2. In Button component, add OnClick event
3. Drag GameOverManager GameObject to the object field
4. Select `GameOverManager.QuitGame` from the function dropdown

### 7. Update UIManager
Assign the following components in UIManager:

**UI Panels:**
- `inGameUIPanel` - Reference to the panel containing all in-game UI elements (Round, Score, Health, Enemies, High Score)

**Game UI Text Elements:**
- `roundText` - Displays current round
- `scoreText` - Displays current score  
- `healthText` - Displays player health
- `enemiesText` - Displays remaining enemies
- `highScoreText` - Displays current high score during gameplay

**Game Over Panel Text Elements:**
- `gameOverScoreText` - Shows final score on game over
- `gameOverHighScoreText` - Shows high score on game over

## How It Works

### Game Over Flow
1. **Player Dies**: Health reaches 0
2. **Score Saved**: Final score is sent to GameOverManager
3. **High Score Check**: If final score > high score, update it
4. **UI Display**: Game over panel shows with scores
5. **Restart Option**: Player can restart the game

### High Score Persistence
- Stored in a ScriptableObject asset
- Persists between play sessions in the Unity Editor
- In builds, persists as long as the asset exists
- Can be reset manually by right-clicking the asset > Reset High Score

### Code Changes Made

#### Player.cs
- `OnDeath()` now calls `GameOverManager.Instance.TriggerGameOver(finalScore)`
- Passes current score from RoundManager to GameOverManager
- Deactivates player GameObject

#### UIManager.cs
- Added `inGameUIPanel` reference to control visibility of in-game UI
- `OnMapGenerated()` ensures in-game UI is visible at game start
- `OnGameOver()` hides in-game UI panel when player dies
- Added `highScoreText` to display high score during gameplay
- Added `gameOverScoreText` and `gameOverHighScoreText` for game over screen
- Subscribes to `GameOverManager.GameOver` event
- Updates game over UI when player dies

#### GameOverManager.cs (New)
- Manages game over state
- Stores high score in GameData ScriptableObject
- Shows/hides game over panel
- Provides restart and quit functionality

#### GameData.cs (New)
- ScriptableObject for persistent data
- Automatically updates high score when beaten
- Can be inspected and modified in the editor

## Testing

1. Play the game
2. Get killed by an enemy
3. Observe:
   - Game over panel appears
   - Final score is displayed
   - High score is displayed
   - Restart button is functional
4. Click Restart
5. Play again and get a higher score
6. Observe that high score updates

## Additional Features to Consider

### Enhanced High Score
- Add player name input
- Add date/time of high score
- Top 10 leaderboard instead of single high score

### Game Over Effects
- Fade to game over screen
- Sound effects for game over
- Particle effects on death

### Statistics
- Track total games played
- Track total enemies killed
- Track highest round reached
- Track total time played

### Save System
- Use PlayerPrefs for build persistence
- JSON serialization for complex data
- Cloud save integration

## Example UI Layout

```
+----------------------------------+
|  Round: 5        High Score: 250 |
|  Score: 180      Enemies: 3      |
|  Health: 15/20                   |
|                                  |
|          [Game Area]             |
|                                  |
+----------------------------------+

// On Game Over:

+----------------------------------+
|         GAME OVER!               |
|                                  |
|    Final Score: 180              |
|    High Score: 250               |
|                                  |
|    [  Restart  ]                 |
|    [   Quit    ]                 |
|                                  |
+----------------------------------+
```

## Troubleshooting

**High Score Not Saving:**
- Ensure GameData ScriptableObject is assigned in GameOverManager
- Check that the asset file exists in the project

**Restart Not Working:**
- Ensure the scene is saved and in Build Settings
- Check button onClick event is correctly assigned

**Game Over Panel Not Appearing:**
- Ensure gameOverPanel is assigned in GameOverManager
- Check that panel is a child of Canvas
- Verify panel starts as inactive (unchecked in hierarchy)
