<?php
// upload_score.php
// Place this file on your web server (e.g., public_html/upload_score.php)

// Enable error reporting for debugging (disable in production)
error_reporting(E_ALL);
ini_set('display_errors', 1);

// Handle OPTIONS preflight request for CORS (important for WebGL builds)
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
    header('Access-Control-Allow-Headers: Content-Type');
    http_response_code(200);
    exit;
}

// Set headers for CORS
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST');
header('Access-Control-Allow-Headers: Content-Type');
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
        'message' => 'Database connection failed: ' . $conn->connect_error
    ]));
}

// Get POST data
$playerName = isset($_POST['playerName']) ? $_POST['playerName'] : '';
$score = isset($_POST['score']) ? intval($_POST['score']) : 0;
$timestamp = isset($_POST['timestamp']) ? $_POST['timestamp'] : date('Y-m-d H:i:s');

// Validate input
if (empty($playerName) || $score < 0) {
    echo json_encode([
        'status' => 'error',
        'message' => 'Invalid input data'
    ]);
    $conn->close();
    exit;
}

// Sanitize input to prevent SQL injection
$playerName = $conn->real_escape_string($playerName);
$timestamp = $conn->real_escape_string($timestamp);

// Optional: Get IP address for tracking
$ip_address = $_SERVER['REMOTE_ADDR'];

// Insert score into database
$sql = "INSERT INTO highscores (player_name, score, timestamp, ip_address) 
        VALUES ('$playerName', $score, '$timestamp', '$ip_address')";

if ($conn->query($sql) === TRUE) {
    echo json_encode([
        'status' => 'success',
        'message' => 'Score uploaded successfully',
        'score_id' => $conn->insert_id,
        'player_name' => $playerName,
        'score' => $score
    ]);
} else {
    echo json_encode([
        'status' => 'error',
        'message' => 'Database error: ' . $conn->error
    ]);
}

$conn->close();
?>
