<?php
// get_leaderboard.php
// Place this file on your web server (e.g., public_html/get_leaderboard.php)

// Handle CORS for WebGL
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
    header('Access-Control-Allow-Headers: Content-Type');
    http_response_code(200);
    exit;
}

header('Access-Control-Allow-Origin: *');
header('Content-Type: application/json');

// Database configuration
$servername = "localhost";
$username = "your_db_username";        // Change this
$password = "your_db_password";        // Change this
$dbname = "game_highscores";          // Change this if needed

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die(json_encode([
        'status' => 'error',
        'message' => 'Connection failed'
    ]));
}

// Get number of scores to return (default 10)
$limit = isset($_GET['limit']) ? intval($_GET['limit']) : 10;
$limit = min($limit, 100); // Cap at 100 for safety

// Query to get top scores
$sql = "SELECT player_name, score, timestamp 
        FROM highscores 
        ORDER BY score DESC 
        LIMIT $limit";

$result = $conn->query($sql);

$leaderboard = [];
if ($result->num_rows > 0) {
    $rank = 1;
    while($row = $result->fetch_assoc()) {
        $leaderboard[] = [
            'rank' => $rank++,
            'player_name' => $row['player_name'],
            'score' => intval($row['score']),
            'timestamp' => $row['timestamp']
        ];
    }
}

echo json_encode([
    'status' => 'success',
    'count' => count($leaderboard),
    'data' => $leaderboard
]);

$conn->close();
?>
