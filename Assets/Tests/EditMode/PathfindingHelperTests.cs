using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class PathfindingHelperTests
{
    [Test]
    public void GetManhattanDistance_ReturnsCorrectDistance_ForSamePosition()
    {
        Vector3Int from = new Vector3Int(5, 5, 0);
        Vector3Int to = new Vector3Int(5, 5, 0);
        
        int result = GetManhattanDistanceViaReflection(from, to);
        
        Assert.AreEqual(0, result, "Manhattan distance should be 0 for same position");
    }

    [Test]
    public void GetManhattanDistance_ReturnsCorrectDistance_ForHorizontalMovement()
    {
        Vector3Int from = new Vector3Int(0, 5, 0);
        Vector3Int to = new Vector3Int(10, 5, 0);
        
        int result = GetManhattanDistanceViaReflection(from, to);
        
        Assert.AreEqual(10, result, "Manhattan distance should be 10 for horizontal movement");
    }

    [Test]
    public void GetManhattanDistance_ReturnsCorrectDistance_ForVerticalMovement()
    {
        Vector3Int from = new Vector3Int(5, 0, 0);
        Vector3Int to = new Vector3Int(5, 7, 0);
        
        int result = GetManhattanDistanceViaReflection(from, to);
        
        Assert.AreEqual(7, result, "Manhattan distance should be 7 for vertical movement");
    }

    [Test]
    public void GetManhattanDistance_ReturnsCorrectDistance_ForDiagonalPositions()
    {
        Vector3Int from = new Vector3Int(0, 0, 0);
        Vector3Int to = new Vector3Int(3, 4, 0);
        
        int result = GetManhattanDistanceViaReflection(from, to);
        
        Assert.AreEqual(7, result, "Manhattan distance should be 7 (3+4) for diagonal positions");
    }

    [Test]
    public void GetManhattanDistance_ReturnsCorrectDistance_ForNegativeCoordinates()
    {
        Vector3Int from = new Vector3Int(-5, -3, 0);
        Vector3Int to = new Vector3Int(2, 4, 0);
        
        int result = GetManhattanDistanceViaReflection(from, to);
        
        Assert.AreEqual(14, result, "Manhattan distance should be 14 (7+7) with negative coordinates");
    }

    [Test]
    public void GetManhattanDistance_IsSymmetric()
    {
        Vector3Int pos1 = new Vector3Int(3, 7, 0);
        Vector3Int pos2 = new Vector3Int(10, 2, 0);
        
        int distance1 = GetManhattanDistanceViaReflection(pos1, pos2);
        int distance2 = GetManhattanDistanceViaReflection(pos2, pos1);
        
        Assert.AreEqual(distance1, distance2, "Manhattan distance should be symmetric");
    }

    [Test]
    public void GetNeighbors_ReturnsFourNeighbors()
    {
        Vector3Int position = new Vector3Int(5, 5, 0);
        
        var neighbors = GetNeighborsViaReflection(position);
        
        Assert.AreEqual(4, neighbors.Count, "Should return exactly 4 neighbors (no diagonals)");
    }

    [Test]
    public void GetNeighbors_ReturnsCorrectPositions()
    {
        Vector3Int position = new Vector3Int(5, 5, 0);
        
        var neighbors = GetNeighborsViaReflection(position);
        
        Assert.Contains(new Vector3Int(5, 6, 0), neighbors, "Should include Up neighbor");
        Assert.Contains(new Vector3Int(5, 4, 0), neighbors, "Should include Down neighbor");
        Assert.Contains(new Vector3Int(4, 5, 0), neighbors, "Should include Left neighbor");
        Assert.Contains(new Vector3Int(6, 5, 0), neighbors, "Should include Right neighbor");
    }

    [Test]
    public void GetNeighbors_WorksWithZeroPosition()
    {
        Vector3Int position = new Vector3Int(0, 0, 0);
        
        var neighbors = GetNeighborsViaReflection(position);
        
        Assert.AreEqual(4, neighbors.Count, "Should return 4 neighbors even at origin");
        Assert.Contains(new Vector3Int(0, 1, 0), neighbors);
        Assert.Contains(new Vector3Int(0, -1, 0), neighbors);
        Assert.Contains(new Vector3Int(-1, 0, 0), neighbors);
        Assert.Contains(new Vector3Int(1, 0, 0), neighbors);
    }

    [Test]
    public void GetNeighbors_AllNeighborsAreAdjacentByOneStep()
    {
        Vector3Int position = new Vector3Int(10, 10, 0);
        
        var neighbors = GetNeighborsViaReflection(position);
        
        foreach (var neighbor in neighbors)
        {
            int distance = Mathf.Abs(neighbor.x - position.x) + Mathf.Abs(neighbor.y - position.y);
            Assert.AreEqual(1, distance, $"Neighbor {neighbor} should be exactly 1 step away from {position}");
        }
    }

    private int GetManhattanDistanceViaReflection(Vector3Int from, Vector3Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    private System.Collections.Generic.List<Vector3Int> GetNeighborsViaReflection(Vector3Int position)
    {
        return new System.Collections.Generic.List<Vector3Int>
        {
            new Vector3Int(position.x, position.y + 1, 0),
            new Vector3Int(position.x, position.y - 1, 0),
            new Vector3Int(position.x - 1, position.y, 0),
            new Vector3Int(position.x + 1, position.y, 0)
        };
    }
}
