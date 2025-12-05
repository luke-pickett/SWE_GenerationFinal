using UnityEngine;

public class PathingNode
{
    public Vector3Int Position { get; set; }
    public int DistanceFromStart { get; set; }
    public int EstimatedDistanceToEnd { get; set; }
    public int TotalCost => DistanceFromStart + EstimatedDistanceToEnd;
    public PathingNode CameFromNode { get; set; }

    public PathingNode(Vector3Int position, int distanceFromStart, int estimatedDistanceToEnd, PathingNode cameFromNode)
    {
        Position = position;
        DistanceFromStart = distanceFromStart;
        EstimatedDistanceToEnd = estimatedDistanceToEnd;
        CameFromNode = cameFromNode;
    }
}
