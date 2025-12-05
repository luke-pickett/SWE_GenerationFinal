# Turn-Based Grid RPG

A turn-based roguelike game built in Unity featuring procedurally generated maps, pathfinding AI enemies, and round-based progression.

## Overview

This is a 2D grid-based turn-based RPG where the player navigates procedurally generated levels, fights enemies, and progresses through increasingly difficult rounds. The game features:

- **Procedural Level Generation**: Each round generates a unique level layout
- **Turn-Based Combat**: Strategic player vs AI enemy combat system
- **A* Pathfinding**: Intelligent enemy AI that navigates to the player
- **Round Progression**: Increasing difficulty with more enemies each round
- **Score System**: Track your performance with persistent high scores
- **Online Leaderboards**: Upload your high scores to compete globally

## How to run
1. Clone the repository
2. Open the project in Unity
3. Go to `File > Build Settings`, select your platform, and click `Build and Run`

## Dependencies
The game uses Unity 6.2 but besides that has no external dependencies.

## High Score Upload System

This game includes an online high score system that allows players to upload their scores to a MySQL database.

### Quick Setup:
1. Follow the instructions in `HIGH_SCORE_UPLOAD_SETUP.md`
2. Configure your server with the PHP files in `ServerFiles/`
3. Set up the MySQL database using `ServerFiles/database_setup.sql`
4. Update the server URLs in the Unity Inspector for the `HighScoreUploader` component

### Files:
- `Assets/Systems/HighScoreUploader.cs` - Unity script for uploading scores
- `ServerFiles/upload_score.php` - PHP endpoint for score uploads
- `ServerFiles/get_leaderboard.php` - PHP endpoint for retrieving leaderboard
- `ServerFiles/database_setup.sql` - Database schema
- `HIGH_SCORE_UPLOAD_SETUP.md` - Complete setup guide

For detailed setup instructions, see [HIGH_SCORE_UPLOAD_SETUP.md](HIGH_SCORE_UPLOAD_SETUP.md)