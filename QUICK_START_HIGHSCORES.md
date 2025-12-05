# High Score Upload - Quick Start Checklist

## Server Setup (Do Once)

### 1. Database Setup
- [ ] Access your MySQL database (phpMyAdmin, cPanel, etc.)
- [ ] Create database: `game_highscores`
- [ ] Run `ServerFiles/database_setup.sql`
- [ ] Verify: `SELECT * FROM highscores;`

### 2. PHP File Upload
- [ ] Upload `upload_score.php` to your web server
- [ ] Upload `get_leaderboard.php` to your web server
- [ ] Edit both files with your database credentials:
  ```php
  $username = "your_actual_username";
  $password = "your_actual_password";
  ```
- [ ] Test in browser: `https://yourdomain.com/get_leaderboard.php`
  - Should show JSON with test data

### 3. Get Your URLs
- [ ] Note your upload URL: `https://yourdomain.com/upload_score.php`
- [ ] Note your leaderboard URL: `https://yourdomain.com/get_leaderboard.php`

---

## Unity Setup (Do Once Per Project)

### 1. Add HighScoreUploader to Scene
- [ ] Create Empty GameObject named "HighScoreUploader"
- [ ] Add Component ? `HighScoreUploader` script
- [ ] Set **Php Upload URL** in Inspector
- [ ] Set **Php Leaderboard URL** in Inspector

### 2. Update Game Over Panel
- [ ] Add Button: "Upload Score"
- [ ] Add TextMeshProUGUI: "UploadStatusText"
- [ ] Drag button to HighScoreUploader's **Upload Button** field
- [ ] Drag text to HighScoreUploader's **Status Text** field

### 3. Link to GameOverManager
- [ ] Find GameOverManager in scene
- [ ] Drag HighScoreUploader to **High Score Uploader** field

### 4. Optional: Add Player Name Input
- [ ] Add Input Field to Game Over Panel
- [ ] Set placeholder: "Enter your name..."
- [ ] Add script to save name using `uploader.SetPlayerName()`

---

## Testing

### Test in Unity Editor
- [ ] Enter Play Mode
- [ ] Trigger Game Over (let player die)
- [ ] Click "Upload Score" button
- [ ] Check Console for: "Score uploaded successfully!"
- [ ] Check database: `SELECT * FROM highscores ORDER BY timestamp DESC;`

### Test in WebGL Build
- [ ] Build for WebGL platform
- [ ] Open in browser
- [ ] Open Browser Console (F12)
- [ ] Trigger Game Over
- [ ] Click Upload
- [ ] Check Network tab for successful request
- [ ] Verify score in database

---

## Common Issues

### ? "Database connection failed"
? Check database credentials in PHP files

### ? "CORS policy blocked"
? Verify CORS headers in PHP files (already included)

### ? "Upload failed: Connection refused"
? Check server URL is correct and accessible

### ? Button doesn't do anything
? Check HighScoreUploader is assigned in GameOverManager

### ? "GridManager.Instance is null"
? Make sure game is running when button is clicked

---

## Quick Reference

### Unity Component Fields
```
HighScoreUploader:
  ?? Php Upload URL: https://yourdomain.com/upload_score.php
  ?? Php Leaderboard URL: https://yourdomain.com/get_leaderboard.php
  ?? Upload Button: [Drag Upload Button here]
  ?? Status Text: [Drag Status Text here]

GameOverManager:
  ?? High Score Uploader: [Drag HighScoreUploader GameObject here]
```

### PHP Database Credentials
```php
// In both upload_score.php and get_leaderboard.php
$servername = "localhost";
$username = "your_mysql_username";
$password = "your_mysql_password";
$dbname = "game_highscores";
```

### SQL Quick Queries
```sql
-- View all scores
SELECT * FROM highscores ORDER BY score DESC LIMIT 10;

-- View recent uploads
SELECT * FROM highscores ORDER BY timestamp DESC LIMIT 10;

-- Delete a score
DELETE FROM highscores WHERE id = 123;

-- Clear all test data
DELETE FROM highscores WHERE player_name = 'TestPlayer1';
```

---

## Security Checklist (Production)

- [ ] Change database username/password from defaults
- [ ] Add rate limiting to PHP scripts
- [ ] Implement API key authentication
- [ ] Use prepared statements (example in setup guide)
- [ ] Set max score limit to prevent cheating
- [ ] Enable HTTPS for both game and API
- [ ] Disable error display in PHP (`display_errors = 0`)
- [ ] Regular database backups
- [ ] Monitor for suspicious activity

---

## Need Help?

1. Check `HIGH_SCORE_UPLOAD_SETUP.md` for detailed instructions
2. Check Unity Console for error messages
3. Check browser Console (F12) for network errors
4. Check server PHP error logs
5. Test endpoints with browser or cURL

---

**Once setup is complete, your players can upload scores and compete on the leaderboard!** ??
