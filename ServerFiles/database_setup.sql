-- database_setup.sql
-- SQL script to create the high scores database and table

-- Create database
CREATE DATABASE IF NOT EXISTS game_highscores;
USE game_highscores;

-- Create highscores table
CREATE TABLE IF NOT EXISTS highscores (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_name VARCHAR(50) NOT NULL,
    score INT NOT NULL,
    timestamp DATETIME NOT NULL,
    ip_address VARCHAR(45),
    INDEX idx_score (score DESC),
    INDEX idx_timestamp (timestamp DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Optional: Create a view for top scores
CREATE OR REPLACE VIEW top_scores AS
SELECT 
    player_name,
    MAX(score) as best_score,
    COUNT(*) as total_games,
    MAX(timestamp) as last_played
FROM highscores
GROUP BY player_name
ORDER BY best_score DESC
LIMIT 100;

-- Optional: Add some sample data for testing
INSERT INTO highscores (player_name, score, timestamp, ip_address) VALUES
('TestPlayer1', 1000, NOW(), '127.0.0.1'),
('TestPlayer2', 850, NOW(), '127.0.0.1'),
('TestPlayer3', 750, NOW(), '127.0.0.1');

-- Query to view all scores
-- SELECT * FROM highscores ORDER BY score DESC LIMIT 10;

-- Query to view top scores per player
-- SELECT * FROM top_scores LIMIT 10;

-- Query to delete old scores (optional maintenance)
-- DELETE FROM highscores WHERE timestamp < DATE_SUB(NOW(), INTERVAL 90 DAY);
