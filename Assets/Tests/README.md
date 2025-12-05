# Pathfinding Unit Tests Setup Guide

## Prerequisites

1. **Install Unity Test Framework**
   - Open Unity Editor
   - Go to Window ? Package Manager
   - Search for "Test Framework"
   - Click Install

## Running the Tests

### In Unity Test Runner:
1. Open Window ? General ? Test Runner
2. Select "PlayMode" tab for integration tests
3. Select "EditMode" tab for unit tests
4. Click "Run All" or select individual tests

### Test Files Created:

#### `Assets/Tests/PlayMode/PathfindingServiceTests.cs`
Integration tests that verify pathfinding behavior with actual GridManager integration.

**Tests Include:**
- ? `GetPath_ReturnsStraightPath_WhenNoObstacles` - Verifies straight-line pathfinding
- ? `GetPath_ReturnsNull_WhenNoPathExists` - Tests blocked path detection  
- ? `GetPath_FindsPathAroundObstacle` - Tests obstacle avoidance
- ? `GetPath_ReturnsOptimalPath_WithMultipleRoutes` - Validates A* optimality
- ? `GetPath_ReturnsSingleTile_WhenStartEqualsEnd` - Edge case testing
- ? `GetPath_WorksWithAllowEmptyTiles` - Tests empty tile traversal mode
- ? `GetPath_RespectsCustomTileCosts` - Tests weighted pathfinding
- ? `GetPath_HandlesLargeGrid` - Performance test (20x20 grid)
- ? `GetPath_HandlesComplexMaze` - Tests maze navigation
- ? `GetPath_NoDiagonalMovement` - Validates 4-directional movement

#### `Assets/Tests/EditMode/PathfindingHelperTests.cs`
Unit tests for pathfinding helper methods (Manhattan distance, neighbors).

**Tests Include:**
- ? `GetManhattanDistance_ReturnsCorrectDistance_ForSamePosition`
- ? `GetManhattanDistance_ReturnsCorrectDistance_ForHorizontalMovement`
- ? `GetManhattanDistance_ReturnsCorrectDistance_ForVerticalMovement`
- ? `GetManhattanDistance_ReturnsCorrectDistance_ForDiagonalPositions`
- ? `GetManhattanDistance_ReturnsCorrectDistance_ForNegativeCoordinates`
- ? `GetManhattanDistance_IsSymmetric`
- ? `GetNeighbors_ReturnsFourNeighbors`
- ? `GetNeighbors_ReturnsCorrectPositions`
- ? `GetNeighbors_WorksWithZeroPosition`
- ? `GetNeighbors_AllNeighborsAreAdjacentByOneStep`

## Test Coverage

### Core Pathfinding Features:
- ? Basic A* pathfinding algorithm
- ? Manhattan distance heuristic
- ? 4-directional movement (no diagonals)
- ? Obstacle detection and avoidance
- ? Optimal path selection
- ? Edge case handling (same start/end, no path, etc.)
- ? Custom tile costs
- ? Empty tile traversal mode
- ? Large grid performance
- ? Complex maze navigation

### Expected Test Results:

**All Tests Should Pass** if:
1. PathfindingService correctly implements A* algorithm
2. GridManager properly tracks walkable tiles
3. No diagonal movement is allowed
4. Manhattan distance is used as heuristic
5. Custom tile costs are respected

## Common Issues & Solutions

### Issue: Tests fail to compile
**Solution:** Install Unity Test Framework via Package Manager

### Issue: "GridManager.Instance is null"
**Solution:** Tests create their own GridManager instance in SetUp

### Issue: Tests timeout
**Solution:** Increase timeout in Test Runner settings for large grid tests

### Issue: Path not optimal
**Solution:** Verify A* implementation uses correct heuristic and cost calculations

## Manual Testing Checklist

If automated tests aren't available, verify manually:

1. **Straight Path Test:**
   - Place start and end 5 tiles apart horizontally
   - Path should be exactly 6 tiles long (0,1,2,3,4,5)

2. **Obstacle Avoidance Test:**
   - Create a wall between start and end
   - Path should go around, not through

3. **No Path Test:**
   - Surround end position with walls
   - Should return null or empty path

4. **Optimal Path Test:**
   - Create multiple possible routes
   - Path should take shortest route

5. **No Diagonal Test:**
   - Every step should change X OR Y by 1, never both

## Running Tests from Command Line

```bash
# Run all PlayMode tests
Unity.exe -runTests -batchmode -projectPath "C:/YourProject" -testPlatform playmode

# Run all EditMode tests  
Unity.exe -runTests -batchmode -projectPath "C:/YourProject" -testPlatform editmode
```

## Test Metrics

**Expected Performance:**
- EditMode tests: < 1 second total
- PlayMode tests: < 30 seconds total
- Individual test: < 3 seconds

**Code Coverage Goal:**
- PathfindingService: 90%+ coverage
- Helper methods: 100% coverage

## Next Steps

1. Install Unity Test Framework
2. Open Test Runner window
3. Run all tests to verify pathfinding correctness
4. Use failed tests to identify and fix bugs
5. Take screenshots of Test Runner showing passing tests for documentation
