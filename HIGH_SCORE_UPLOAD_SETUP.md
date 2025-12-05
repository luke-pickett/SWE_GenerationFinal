# High Score Upload System Setup Guide

## Overview

This system allows players to upload their high scores to a MySQL database via PHP scripts. It supports both standalone and WebGL builds with full CORS support.

---

## Table of Contents

1. [File Structure](#file-structure)
2. [Server Setup](#server-setup)
3. [Unity Setup](#unity-setup)
4. [Testing](#testing)
5. [WebGL Deployment](#webgl-deployment)
6. [Troubleshooting](#troubleshooting)

---

## File Structure

```
SWE_GenerationFinal/
??? Assets/
?   ??? Systems/
?       ??? HighScoreUploader.cs      (NEW - Unity script)
?       ??? GameOverManager.cs        (UPDATED)
?
??? ServerFiles/
    ??? upload_score.php              (NEW - Upload endpoint)
    ??? get_leaderboard.php          (NEW - Leaderboard endpoint)
    ??? database_setup.sql           (NEW - Database schema)
```

---

## Server Setup

### Step 1: Database Setup

1. **Access your MySQL database** (via phpMyAdmin, cPanel, or command line)

2. **Run the SQL script:**
   - Open `ServerFiles/database_setup.sql`
   - Execute the entire script in your MySQL database
   - This will create:
     - Database: `game_highscores`
     - Table: `highscores`
     - Sample test data

3. **Verify creation:**
```sql
USE game_highscores;
SHOW TABLES;
SELECT * FROM highscores;
```

### Step 2: Upload PHP Files

1. **Upload to your web server:**
   - Copy `upload_score.php` to your web server (e.g., `public_html/`)
   - Copy `get_leaderboard.php` to the same directory

2. **Configure database credentials** in both PHP files:
```php
$servername = "localhost";           // Usually "localhost"
$username = "your_db_username";      // Your MySQL username
$password = "your_db_password";      // Your MySQL password
$dbname = "game_highscores";        // Database name
```

3. **Set file permissions:**
```bash
chmod 644 upload_score.php
chmod 644 get_leaderboard.php
```

### Step 3: Test PHP Scripts

1. **Test upload script:**
   - Open in browser: `https://yourdomain.com/upload_score.php`
   - Should show an error (expected - needs POST data)

2. **Test leaderboard script:**
   - Open in browser: `https://yourdomain.com/get_leaderboard.php`
   - Should display JSON with test scores:
```json
{
  "status": "success",
  "count": 3,
  "data": [
    {
      "rank": 1,
      "player_name": "TestPlayer1",
      "score": 1000,
      "timestamp": "2024-01-01 12:00:00"
    }
  ]
}
```

---

## Unity Setup

### Step 1: Add HighScoreUploader to Scene

1. **Open your main game scene** in Unity

2. **Create a new GameObject:**
   - Right-click in Hierarchy ? Create Empty
   - Name it: `HighScoreUploader`

3. **Add the component:**
   - Select the `HighScoreUploader` GameObject
   - Add Component ? Scripts ? `HighScoreUploader`

4. **Configure the URLs:**
   - In Inspector, set:
     - **Php Upload URL**: `https://yourdomain.com/upload_score.php`
     - **Php Leaderboard URL**: `https://yourdomain.com/get_leaderboard.php`

### Step 2: Update Game Over Panel UI

1. **Open your Game Over Panel prefab/scene object**

2. **Add an Upload Button:**
   - Right-click on Game Over Panel ? UI ? Button - TextMeshPro
   - Name it: `UploadScoreButton`
   - Change button text to: "Upload Score"

3. **Add a Status Text:**
   - Right-click on Game Over Panel ? UI ? Text - TextMeshPro
   - Name it: `UploadStatusText`
   - Set default text to: "" (empty)
   - Adjust font size/color as needed

4. **Connect to HighScoreUploader:**
   - Select the `HighScoreUploader` GameObject
   - Drag the `UploadScoreButton` to the **Upload Button** field
   - Drag the `UploadStatusText` to the **Status Text** field

### Step 3: Link to GameOverManager

1. **Find GameOverManager** in your scene

2. **Connect the uploader:**
   - Select `GameOverManager` GameObject
   - Find the **High Score Upload** section
   - Drag the `HighScoreUploader` GameObject to the **High Score Uploader** field

3. **Optional: Add button click event**
   - Select `UploadScoreButton`
   - In Inspector, find **On Click ()** event
   - Click `+` to add event
   - Drag `GameOverManager` to the object field
   - Select: `GameOverManager.UploadHighScore()`

### Step 4: Player Name Setup (Optional)

Add a name input field to allow players to enter their name:

1. **Create Input Field:**
   - In Game Over Panel ? UI ? Input Field - TextMeshPro
   - Name it: `PlayerNameInput`

2. **Add script to handle name:**
```csharp
// Add to your UIManager or create new script
using UnityEngine;
using TMPro;

public class PlayerNameHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private HighScoreUploader uploader;

    private void Start()
    {
        // Load saved name
        if (nameInput != null)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            nameInput.text = savedName;
        }
    }

    public void OnNameChanged()
    {
        if (uploader != null && nameInput != null)
        {
            uploader.SetPlayerName(nameInput.text);
        }
    }
}
```

---

## Testing

### Test in Unity Editor

1. **Enter Play Mode**

2. **Trigger Game Over:**
   - Let your player die
   - Game Over panel should appear

3. **Click Upload Score button:**
   - Watch Console for logs
   - Status text should show: "Uploading..." then "Score uploaded successfully!"

4. **Check database:**
```sql
SELECT * FROM highscores ORDER BY timestamp DESC LIMIT 5;
```

### Test with cURL (Command Line)

```bash
# Test upload
curl -X POST https://yourdomain.com/upload_score.php \
  -d "playerName=TestPlayer" \
  -d "score=999" \
  -d "timestamp=2024-01-01 12:00:00"

# Test leaderboard
curl https://yourdomain.com/get_leaderboard.php?limit=10
```

### Common Issues During Testing

#### Issue: "Database connection failed"
**Solution:** Check database credentials in PHP files

#### Issue: "CORS policy blocked"
**Solution:** Ensure PHP files have proper CORS headers (already included)

#### Issue: "Upload failed: Connection refused"
**Solution:** Verify server URL is correct and accessible

#### Issue: No response in Unity
**Solution:** Check Console for errors, verify `UnityWebRequest` is not blocked by firewall

---

## WebGL Deployment

### Important WebGL Considerations

1. **CORS is critical** - PHP files already include CORS headers
2. **SSL required** - Use HTTPS for both game and API
3. **PlayerPrefs works differently** - Uses browser localStorage

### WebGL Build Settings

1. **Build Settings:**
   - File ? Build Settings
   - Select **WebGL** platform
   - Click **Switch Platform**

2. **Player Settings:**
   - Edit ? Project Settings ? Player
   - Under **WebGL settings**:
     - Enable **Data Caching**
     - Set **Compression Format** to Gzip or Brotli

3. **Build:**
   - Click **Build and Run**
   - Test upload functionality in browser

### Testing WebGL Build

1. **Open browser Console** (F12)
2. **Trigger game over**
3. **Click upload**
4. **Check Network tab** for request/response

---

## Security Improvements (Recommended)

### 1. Add Rate Limiting

Add to top of `upload_score.php`:

```php
session_start();
$time_limit = 60; // seconds
$max_attempts = 5;

if (!isset($_SESSION['upload_attempts'])) {
    $_SESSION['upload_attempts'] = 0;
    $_SESSION['first_attempt_time'] = time();
}

if ($_SESSION['upload_attempts'] >= $max_attempts) {
    $time_passed = time() - $_SESSION['first_attempt_time'];
    if ($time_passed < $time_limit) {
        die(json_encode(['status' => 'error', 'message' => 'Too many attempts']));
    } else {
        $_SESSION['upload_attempts'] = 0;
        $_SESSION['first_attempt_time'] = time();
    }
}

$_SESSION['upload_attempts']++;
```

### 2. Add Score Validation

Prevent impossibly high scores:

```php
// In upload_score.php, after getting $score
$max_allowed_score = 100000; // Set your game's maximum possible score
if ($score > $max_allowed_score) {
    die(json_encode(['status' => 'error', 'message' => 'Invalid score']));
}
```

### 3. Use Prepared Statements

Replace SQL query with prepared statement:

```php
// Better security
$stmt = $conn->prepare("INSERT INTO highscores (player_name, score, timestamp, ip_address) VALUES (?, ?, ?, ?)");
$stmt->bind_param("siss", $playerName, $score, $timestamp, $ip_address);

if ($stmt->execute()) {
    echo json_encode(['status' => 'success', 'score_id' => $stmt->insert_id]);
} else {
    echo json_encode(['status' => 'error', 'message' => $stmt->error]);
}
$stmt->close();
```

### 4. Add API Key Authentication

In Unity:
```csharp
form.AddField("api_key", "your_secret_key");
```

In PHP:
```php
$expected_key = "your_secret_key";
$provided_key = isset($_POST['api_key']) ? $_POST['api_key'] : '';
if ($provided_key !== $expected_key) {
    die(json_encode(['status' => 'error', 'message' => 'Unauthorized']));
}
```

---

## Troubleshooting

### Unity Console Errors

#### "GridManager.Instance is null"
**Cause:** HighScoreUploader trying to access managers before they're initialized  
**Solution:** Ensure GameOverManager is properly set up in scene

#### "UnityWebRequest: Connection refused"
**Cause:** Server URL is wrong or server is down  
**Solution:** 
- Verify URL in HighScoreUploader component
- Test URL in browser
- Check server firewall settings

### PHP Errors

#### "Access denied for user"
**Cause:** Wrong database credentials  
**Solution:** Update username/password in PHP files

#### "Unknown database 'game_highscores'"
**Cause:** Database not created  
**Solution:** Run `database_setup.sql` script

#### "Call to undefined function mysqli_connect()"
**Cause:** MySQLi extension not installed  
**Solution:** Enable in php.ini: `extension=mysqli`

### Database Issues

#### "Table 'highscores' doesn't exist"
**Solution:**
```sql
USE game_highscores;
SOURCE database_setup.sql;
```

#### "Too many connections"
**Solution:** Check that PHP files properly close connections (`$conn->close()`)

---

## Advanced Features

### Add Leaderboard Display in Unity

Create `LeaderboardDisplay.cs`:

```csharp
using System.Collections;
using UnityEngine;
using TMPro;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] private HighScoreUploader uploader;
    [SerializeField] private TextMeshProUGUI leaderboardText;

    public void RefreshLeaderboard()
    {
        StartCoroutine(uploader.FetchLeaderboard(DisplayLeaderboard));
    }

    private void DisplayLeaderboard(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            leaderboardText.text = "Failed to load leaderboard";
            return;
        }

        // Parse JSON and display
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(jsonData);
        
        string display = "=== TOP SCORES ===\n\n";
        foreach (var entry in data.data)
        {
            display += $"{entry.rank}. {entry.player_name} - {entry.score}\n";
        }
        
        leaderboardText.text = display;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public string status;
    public ScoreEntry[] data;
}

[System.Serializable]
public class ScoreEntry
{
    public int rank;
    public string player_name;
    public int score;
    public string timestamp;
}
```

---

## Maintenance

### Clean Old Scores

Run periodically:
```sql
-- Delete scores older than 90 days
DELETE FROM highscores WHERE timestamp < DATE_SUB(NOW(), INTERVAL 90 DAY);

-- Optimize table
OPTIMIZE TABLE highscores;
```

### Backup Database

```bash
mysqldump -u username -p game_highscores > highscores_backup.sql
```

### Monitor Performance

```sql
-- Check table size
SELECT 
    table_name,
    ROUND(((data_length + index_length) / 1024 / 1024), 2) AS "Size (MB)"
FROM information_schema.TABLES
WHERE table_schema = "game_highscores";

-- Check recent uploads
SELECT COUNT(*) as uploads_today 
FROM highscores 
WHERE DATE(timestamp) = CURDATE();
```

---

## Support

For issues or questions:
1. Check Unity Console for error messages
2. Check PHP error logs on server
3. Verify database connection with phpMyAdmin
4. Test endpoints with browser or cURL

---

## Summary Checklist

- [ ] Database created and populated
- [ ] PHP files uploaded and configured
- [ ] HighScoreUploader GameObject added to scene
- [ ] Upload button added to Game Over panel
- [ ] Status text added to Game Over panel
- [ ] Components connected in Inspector
- [ ] Server URLs configured in HighScoreUploader
- [ ] Tested in Unity Editor
- [ ] Tested in WebGL build (if deploying to web)
- [ ] Database showing uploaded scores
- [ ] Security measures implemented

---

**Your high score upload system is now ready!** ????
