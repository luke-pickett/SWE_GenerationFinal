using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class PathfindingServiceTests
{
    private GameObject gridManagerObj;
    private GameObject pathfindingServiceObj;
    private GridManager gridManager;
    private PathfindingService pathfindingService;
    private Grid grid;
    private Tilemap tilemap;
    private TileBase pathableTile;
    private TileBase blockedTile;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        gridManagerObj = new GameObject("GridManager");
        grid = gridManagerObj.AddComponent<Grid>();
        tilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
        tilemap.transform.SetParent(gridManagerObj.transform);
        var tilemapRenderer = tilemap.gameObject.AddComponent<TilemapRenderer>();
        
        gridManager = gridManagerObj.AddComponent<GridManager>();
        
        pathableTile = ScriptableObject.CreateInstance<Tile>();
        blockedTile = ScriptableObject.CreateInstance<Tile>();
        
        typeof(GridManager).GetField("parentGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gridManager, grid);
        typeof(GridManager).GetField("pathingTilemap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gridManager, tilemap);
        typeof(GridManager).GetField("pathableTile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gridManager, pathableTile);
        typeof(GridManager).GetField("blockedTile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gridManager, blockedTile);
        
        pathfindingServiceObj = new GameObject("PathfindingService");
        pathfindingService = pathfindingServiceObj.AddComponent<PathfindingService>();
        
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(gridManagerObj);
        Object.Destroy(pathfindingServiceObj);
        Object.Destroy(pathableTile);
        Object.Destroy(blockedTile);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GetPath_ReturnsStraightPath_WhenNoObstacles()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(5, 0, 0);
        
        for (int x = 0; x <= 5; x++)
        {
            gridManager.SetTile(new Vector3Int(x, 0, 0), pathableTile);
        }
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNotNull(path, "Path should not be null");
        Assert.AreEqual(6, path.Count, "Path should have 6 tiles (0 to 5 inclusive)");
        Assert.AreEqual(start, path[0], "Path should start at start position");
        Assert.AreEqual(end, path[path.Count - 1], "Path should end at end position");
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            int distance = Mathf.Abs(path[i].x - path[i + 1].x) + Mathf.Abs(path[i].y - path[i + 1].y);
            Assert.AreEqual(1, distance, "Each step should move exactly 1 tile (no diagonals)");
        }
    }

    [UnityTest]
    public IEnumerator GetPath_ReturnsNull_WhenNoPathExists()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(5, 0, 0);
        
        gridManager.SetTile(start, pathableTile);
        gridManager.SetTile(end, pathableTile);
        
        for (int x = 1; x < 5; x++)
        {
            gridManager.SetTile(new Vector3Int(x, 0, 0), blockedTile);
        }
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNull(path, "Path should be null when no path exists");
    }

    [UnityTest]
    public IEnumerator GetPath_FindsPathAroundObstacle()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(4, 0, 0);
        
        for (int x = 0; x <= 4; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                gridManager.SetTile(new Vector3Int(x, y, 0), pathableTile);
            }
        }
        
        gridManager.SetTile(new Vector3Int(2, 0, 0), blockedTile);
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNotNull(path, "Path should exist around obstacle");
        Assert.AreEqual(start, path[0], "Path should start at start position");
        Assert.AreEqual(end, path[path.Count - 1], "Path should end at end position");
        Assert.IsFalse(path.Contains(new Vector3Int(2, 0, 0)), "Path should not go through blocked tile");
    }

    [UnityTest]
    public IEnumerator GetPath_ReturnsOptimalPath_WithMultipleRoutes()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(3, 3, 0);
        
        for (int x = 0; x <= 3; x++)
        {
            for (int y = 0; y <= 3; y++)
            {
                gridManager.SetTile(new Vector3Int(x, y, 0), pathableTile);
            }
        }
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNotNull(path, "Path should exist");
        int manhattanDistance = Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);
        Assert.AreEqual(manhattanDistance + 1, path.Count, "Path should be optimal (Manhattan distance + 1)");
    }

    [UnityTest]
    public IEnumerator GetPath_ReturnsSingleTile_WhenStartEqualsEnd()
    {
        Vector3Int position = new Vector3Int(0, 0, 0);
        
        gridManager.SetTile(position, pathableTile);
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(position, position);
        
        Assert.IsNotNull(path, "Path should not be null");
        Assert.AreEqual(1, path.Count, "Path should contain only start/end position");
        Assert.AreEqual(position, path[0], "Path should contain the position");
    }

    [UnityTest]
    public IEnumerator GetPath_WorksWithAllowEmptyTiles()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(3, 3, 0);
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end, null, true);
        
        Assert.IsNotNull(path, "Path should exist with allowEmptyTiles");
        Assert.AreEqual(start, path[0], "Path should start at start position");
        Assert.AreEqual(end, path[path.Count - 1], "Path should end at end position");
    }

    [UnityTest]
    public IEnumerator GetPath_RespectsCustomTileCosts()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(2, 0, 0);
        
        TileBase expensiveTile = ScriptableObject.CreateInstance<Tile>();
        
        gridManager.SetTile(new Vector3Int(0, 0, 0), pathableTile);
        gridManager.SetTile(new Vector3Int(1, 0, 0), expensiveTile);
        gridManager.SetTile(new Vector3Int(2, 0, 0), pathableTile);
        gridManager.SetTile(new Vector3Int(0, 1, 0), pathableTile);
        gridManager.SetTile(new Vector3Int(1, 1, 0), pathableTile);
        gridManager.SetTile(new Vector3Int(2, 1, 0), pathableTile);
        
        Dictionary<TileBase, int> tileCosts = new Dictionary<TileBase, int>
        {
            { expensiveTile, 10 }
        };
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end, tileCosts);
        
        Assert.IsNotNull(path, "Path should exist");
        Assert.IsFalse(path.Contains(new Vector3Int(1, 0, 0)), "Path should avoid expensive tile when alternative exists");
        
        Object.Destroy(expensiveTile);
    }

    [UnityTest]
    public IEnumerator GetPath_HandlesLargeGrid()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(20, 20, 0);
        
        for (int x = 0; x <= 20; x++)
        {
            for (int y = 0; y <= 20; y++)
            {
                gridManager.SetTile(new Vector3Int(x, y, 0), pathableTile);
            }
        }
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNotNull(path, "Path should exist in large grid");
        Assert.AreEqual(41, path.Count, "Path should be optimal in large grid");
    }

    [UnityTest]
    public IEnumerator GetPath_HandlesComplexMaze()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(4, 4, 0);
        
        for (int x = 0; x <= 4; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                gridManager.SetTile(new Vector3Int(x, y, 0), pathableTile);
            }
        }
        
        gridManager.SetTile(new Vector3Int(1, 0, 0), blockedTile);
        gridManager.SetTile(new Vector3Int(1, 1, 0), blockedTile);
        gridManager.SetTile(new Vector3Int(1, 2, 0), blockedTile);
        gridManager.SetTile(new Vector3Int(3, 2, 0), blockedTile);
        gridManager.SetTile(new Vector3Int(3, 3, 0), blockedTile);
        gridManager.SetTile(new Vector3Int(3, 4, 0), blockedTile);
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNotNull(path, "Path should exist through maze");
        Assert.AreEqual(start, path[0], "Path should start at start");
        Assert.AreEqual(end, path[path.Count - 1], "Path should end at end");
    }

    [UnityTest]
    public IEnumerator GetPath_NoDiagonalMovement()
    {
        Vector3Int start = new Vector3Int(0, 0, 0);
        Vector3Int end = new Vector3Int(3, 3, 0);
        
        for (int x = 0; x <= 3; x++)
        {
            for (int y = 0; y <= 3; y++)
            {
                gridManager.SetTile(new Vector3Int(x, y, 0), pathableTile);
            }
        }
        
        yield return null;
        
        List<Vector3Int> path = pathfindingService.GetPath(start, end);
        
        Assert.IsNotNull(path, "Path should exist");
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3Int current = path[i];
            Vector3Int next = path[i + 1];
            
            int dx = Mathf.Abs(next.x - current.x);
            int dy = Mathf.Abs(next.y - current.y);
            
            Assert.IsTrue((dx == 1 && dy == 0) || (dx == 0 && dy == 1), 
                "Movement should be only horizontal or vertical, not diagonal");
        }
    }
}
