using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public static Action MapGeneratorInstanced;
    public static Action MapGenerated;

    public static MapGenerator Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TileBase pathableTile;
    [SerializeField] private TileBase blockedTile;

    [Header("Map Settings")]
    [Range(10, 50)]
    [SerializeField] private int mapDimensions;
    [Range(1, 15)]
    [SerializeField] private int iterations;
    [Range(1, 10)]
    [SerializeField] private int iterationSize;
    [Range(1, 5)]
    [SerializeField] private int minConnectionsPerRoom = 1;
    [Range(1, 5)]
    [SerializeField] private int maxConnectionsPerRoom = 3;

    private void OnEnable()
    {
        GameLoop.FireMapGeneration += GenerateMap;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            MapGeneratorInstanced?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private struct Room
    {
        public List<Vector3Int> tilePositions;

        public Room(List<Vector3Int> tilePositions)
        {
            this.tilePositions = tilePositions;
        }
    }

    private List<Room> rooms = new List<Room>();

    private void GenerateMap()
    {
        for (int i = 0; i < iterations; i++)
        {
            Vector3Int randomPosition = new Vector3Int(
                Random.Range(0, mapDimensions + 1),
                Random.Range(0, mapDimensions + 1),
                0
            );
            PerformIteration(randomPosition);
        }
        Debug.Log($"Generated {rooms.Count} rooms.");
        ConnectRooms();
        FillRemainingGrid(10);
        MapGenerated?.Invoke();
    }

    private void PerformIteration(Vector3Int cornerPosition)
    {
        List<Vector3Int> newTilePositions = new List<Vector3Int>();
        for (int i = 0; i < iterationSize; i++)
        {
            for (int j = 0; j < iterationSize; j++)
            {
                Vector3Int tilePosition = new Vector3Int(
                    cornerPosition.x + i,
                    cornerPosition.y - j,
                    0
                );
                GridManager.Instance.SetTile(tilePosition, pathableTile);
                newTilePositions.Add(tilePosition);
            }
        }
        Room iteratedRoom = new Room(newTilePositions);
        foreach (Room existingRoom in rooms.ToArray())
        {
            if (IsRoomAdjacentOrOverlapping(existingRoom, iteratedRoom))
            {
                iteratedRoom = MergeRooms(existingRoom, iteratedRoom);
                rooms.Remove(existingRoom);
            }
        }
        rooms.Add(iteratedRoom);
    }

    private bool IsRoomAdjacentOrOverlapping(Room roomA, Room roomB)
    {
        foreach (Vector3Int posA in roomA.tilePositions)
        {
            foreach (Vector3Int posB in roomB.tilePositions)
            {
                if (Vector3Int.Distance(posA, posB) <= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private Room MergeRooms(Room roomA, Room roomB)
    {
        Room mergedRoom = new Room
        {
            tilePositions = new List<Vector3Int>(roomA.tilePositions)
        };
        foreach (Vector3Int pos in roomB.tilePositions)
        {
            if (!mergedRoom.tilePositions.Contains(pos))
            {
                mergedRoom.tilePositions.Add(pos);
            }
        }
        return mergedRoom;
    }

    private void ConnectRooms()
    {
        if (rooms.Count <= 1)
        {
            return;
        }

        Dictionary<Room, int> roomConnections = new Dictionary<Room, int>();
        Dictionary<Room, int> targetConnections = new Dictionary<Room, int>();
        
        foreach (Room room in rooms)
        {
            roomConnections[room] = 0;
            targetConnections[room] = Random.Range(minConnectionsPerRoom, maxConnectionsPerRoom + 1);
        }

        HashSet<Room> connectedRooms = new HashSet<Room>();
        connectedRooms.Add(rooms[0]);

        while (connectedRooms.Count < rooms.Count)
        {
            Vector3Int bestStart = Vector3Int.zero;
            Vector3Int bestEnd = Vector3Int.zero;
            float shortestDistance = float.MaxValue;
            Room bestConnectedRoom = default;
            Room bestUnconnectedRoom = default;

            foreach (Room connectedRoom in connectedRooms)
            {
                foreach (Room unconnectedRoom in rooms)
                {
                    if (connectedRooms.Contains(unconnectedRoom))
                    {
                        continue;
                    }

                    Vector3Int edgeA = GetRandomEdgeTile(connectedRoom);
                    Vector3Int edgeB = GetRandomEdgeTile(unconnectedRoom);
                    float distance = Vector3Int.Distance(edgeA, edgeB);

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        bestConnectedRoom = connectedRoom;
                        bestUnconnectedRoom = unconnectedRoom;
                        bestStart = edgeA;
                        bestEnd = edgeB;
                    }
                }
            }

            List<Vector3Int> path = PathfindingService.Instance.GetPath(bestStart, bestEnd, null, true);
            if (path != null)
            {
                foreach (Vector3Int position in path)
                {
                    GridManager.Instance.SetTile(position, pathableTile);
                }
                roomConnections[bestConnectedRoom]++;
                roomConnections[bestUnconnectedRoom]++;
                connectedRooms.Add(bestUnconnectedRoom);
                Debug.Log($"Connected room, total connected: {connectedRooms.Count}");
            }
            else
            {
                connectedRooms.Add(bestUnconnectedRoom);
                Debug.LogWarning($"Could not find path between rooms, skipping.");
            }
        }

        List<Room> roomsNeedingConnections = new List<Room>();
        foreach (Room room in rooms)
        {
            if (roomConnections[room] < targetConnections[room])
            {
                roomsNeedingConnections.Add(room);
            }
        }

        foreach (Room room in roomsNeedingConnections)
        {
            int connectionsNeeded = targetConnections[room] - roomConnections[room];
            
            for (int i = 0; i < connectionsNeeded; i++)
            {
                List<Room> potentialTargets = new List<Room>();
                foreach (Room otherRoom in rooms)
                {
                    if (otherRoom.Equals(room))
                    {
                        continue;
                    }
                    if (roomConnections[otherRoom] < targetConnections[otherRoom])
                    {
                        potentialTargets.Add(otherRoom);
                    }
                }

                if (potentialTargets.Count == 0)
                {
                    potentialTargets.AddRange(rooms);
                    potentialTargets.Remove(room);
                }

                if (potentialTargets.Count == 0)
                {
                    break;
                }

                Room targetRoom = potentialTargets[Random.Range(0, potentialTargets.Count)];
                Vector3Int startEdge = GetRandomEdgeTile(room);
                Vector3Int endEdge = GetRandomEdgeTile(targetRoom);

                List<Vector3Int> path = PathfindingService.Instance.GetPath(startEdge, endEdge, null, true);
                if (path != null)
                {
                    foreach (Vector3Int position in path)
                    {
                        GridManager.Instance.SetTile(position, pathableTile);
                    }
                    roomConnections[room]++;
                    roomConnections[targetRoom]++;
                    Debug.Log($"Added additional connection for room");
                }
            }
        }

        Debug.Log($"Room connections complete. Total connections: {GetTotalConnections(roomConnections)}");
    }

    private int GetTotalConnections(Dictionary<Room, int> roomConnections)
    {
        int total = 0;
        foreach (var count in roomConnections.Values)
        {
            total += count;
        }
        return total / 2;
    }

    private Vector3Int GetRandomEdgeTile(Room room)
    {
        List<Vector3Int> edgeTiles = new List<Vector3Int>();

        foreach (Vector3Int tile in room.tilePositions)
        {
            Dictionary<Directions.Direction, Vector3Int> adjacentTiles = GridManager.Instance.GetAdjacentTiles(tile);

            foreach (Vector3Int neighbor in adjacentTiles.Values)
            {
                if (!room.tilePositions.Contains(neighbor))
                {
                    edgeTiles.Add(tile);
                    break;
                }
            }
        }

        if (edgeTiles.Count > 0)
        {
            return edgeTiles[Random.Range(0, edgeTiles.Count)];
        }

        return room.tilePositions[Random.Range(0, room.tilePositions.Count)];
    }

    private void FillRemainingGrid(int padding)
    {
        if (rooms.Count == 0)
        {
            return;
        }

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Room room in rooms)
        {
            foreach (Vector3Int position in room.tilePositions)
            {
                if (position.x < minX) minX = position.x;
                if (position.y < minY) minY = position.y;
                if (position.x > maxX) maxX = position.x;
                if (position.y > maxY) maxY = position.y;
            }
        }

        minX -= padding;
        minY -= padding;
        maxX += padding;
        maxY += padding;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                GridManager.Instance.SetTile(position, blockedTile, false);
            }
        }

        Debug.Log($"Filled grid bounds: ({minX}, {minY}) to ({maxX}, {maxY})");
    }
}
