using System;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingService : MonoBehaviour
{
    public static Action PathfindingServiceInstanced;

    public static PathfindingService Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PathfindingServiceInstanced?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Vector3Int> GetPath(Vector3Int start, Vector3Int end, Dictionary<TileBase, int> tileCosts=null, bool allowEmptyTiles=false)
    {
        if (!ArePositionsValid(start, end, allowEmptyTiles))
        {
            return null;
        }

        List<PathingNode> openSet = new List<PathingNode>();
        Dictionary<Vector3Int, PathingNode> allNodes = new Dictionary<Vector3Int, PathingNode>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        PathingNode startNode = new PathingNode(start, 0, GetManhattanDistance(start, end), null);
        openSet.Add(startNode);
        allNodes[start] = startNode;

        while (openSet.Count > 0)
        {
            PathingNode currentNode = GetLowestCostNode(openSet);

            if (currentNode.Position == end)
            {
                return ReconstructPath(currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            foreach (Vector3Int neighborPosition in GetNeighbors(currentNode.Position))
            {
                if (closedSet.Contains(neighborPosition))
                {
                    continue;
                }

                TileBase neighborTile = GridManager.Instance.GetTile(neighborPosition);
                if (!allowEmptyTiles && neighborTile == null)
                {
                    continue;
                }

                if (!allowEmptyTiles && !GridManager.Instance.IsWalkable(neighborPosition))
                {
                    continue;
                }

                int moveCost = neighborTile != null ? GetTileCost(neighborTile, tileCosts) : 1;
                int tentativeGScore = currentNode.DistanceFromStart + moveCost;

                if (!allNodes.ContainsKey(neighborPosition))
                {
                    PathingNode neighborNode = new PathingNode(
                        neighborPosition,
                        tentativeGScore,
                        GetManhattanDistance(neighborPosition, end),
                        currentNode
                    );
                    allNodes[neighborPosition] = neighborNode;
                    openSet.Add(neighborNode);
                }
                else if (tentativeGScore < allNodes[neighborPosition].DistanceFromStart)
                {
                    PathingNode neighborNode = allNodes[neighborPosition];
                    neighborNode.DistanceFromStart = tentativeGScore;
                    neighborNode.CameFromNode = currentNode;
                }
            }
        }

        return null;
    }

    private bool ArePositionsValid(Vector3Int start, Vector3Int end, bool allowEmptyTiles=true)
    {
        if (!allowEmptyTiles)
        {
            if (GridManager.Instance.GetTile(start) == null ||
                GridManager.Instance.GetTile(end) == null)
            {
                Debug.LogWarning("Start or end position is out of bounds.");
                return false;
            }
        }
        return true;
    }

    private int GetManhattanDistance(Vector3Int from, Vector3Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    private PathingNode GetLowestCostNode(List<PathingNode> nodes)
    {
        PathingNode lowest = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].TotalCost < lowest.TotalCost)
            {
                lowest = nodes[i];
            }
        }
        return lowest;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        return new List<Vector3Int>
        {
            new Vector3Int(position.x, position.y + 1, 0),
            new Vector3Int(position.x, position.y - 1, 0),
            new Vector3Int(position.x - 1, position.y, 0),
            new Vector3Int(position.x + 1, position.y, 0)
        };
    }

    private int GetTileCost(TileBase tile, Dictionary<TileBase, int> tileCosts)
    {
        if (tileCosts != null && tileCosts.ContainsKey(tile))
        {
            return tileCosts[tile];
        }
        return 1;
    }

    private List<Vector3Int> ReconstructPath(PathingNode endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        PathingNode currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.CameFromNode;
        }

        path.Reverse();
        return path;
    }
}
