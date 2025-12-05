using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public static Action GridManagerInstanced;

    public static GridManager Instance { get; private set; }
    [Header("References")]
    [SerializeField] private Grid parentGrid;
    [SerializeField] private Tilemap pathingTilemap;
    [SerializeField] private TileBase pathableTile;
    [SerializeField] private TileBase blockedTile;

    public struct TileData
    {
        public Vector3Int position;
        public TileBase tileType;
        public IEntity contains;

        public TileData(Vector3Int position, TileBase tileType, IEntity contains = null)
        {
            this.position = position;
            this.tileType = tileType;
            this.contains = contains;
        }
    }

    private Dictionary<Vector3Int, TileData> _tileDataMap = new Dictionary<Vector3Int, TileData>();
    private List<TileData> _walkableTiles = new List<TileData>();
    private Vector2Int _gridDimensions = new Vector2Int(int.MinValue, int.MaxValue);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GridManagerInstanced?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTile(Vector3Int position, TileBase tileType, bool overwrite=true)
    {
        if (overwrite || pathingTilemap.GetTile(position) == null)
        {
            pathingTilemap.SetTile(position, tileType);
            
            TileData tileData = new TileData(position, tileType, null);
            if (_tileDataMap.ContainsKey(position))
            {
                TileData existingData = _tileDataMap[position];
                tileData.contains = existingData.contains;
            }
            _tileDataMap[position] = tileData;

            if (position.x > _gridDimensions.x)
            {
                _gridDimensions.x = position.x;
            }
            if (position.y < _gridDimensions.y)
            {
                _gridDimensions.y = position.y;
            }

            if (tileType == pathableTile)
            {
                _walkableTiles.Add(tileData);
            }
        }
    }

    public TileBase GetTile(Vector3Int position)
    {
        return pathingTilemap.GetTile(position);
    }

    public TileData? GetTileData(Vector3Int position)
    {
        if (_tileDataMap.ContainsKey(position))
        {
            return _tileDataMap[position];
        }
        return null;
    }

    public bool HasEntity(Vector3Int position)
    {
        if (_tileDataMap.ContainsKey(position))
        {
            return _tileDataMap[position].contains != null;
        }
        return false;
    }

    public IEntity GetEntity(Vector3Int position)
    {
        if (_tileDataMap.ContainsKey(position))
        {
            return _tileDataMap[position].contains;
        }
        return null;
    }

    public void SetEntity(Vector3Int position, IEntity entity)
    {
        if (_tileDataMap.ContainsKey(position))
        {
            TileData tileData = _tileDataMap[position];
            tileData.contains = entity;
            _tileDataMap[position] = tileData;
        }
    }

    public void RemoveEntity(Vector3Int position)
    {
        if (_tileDataMap.ContainsKey(position))
        {
            TileData tileData = _tileDataMap[position];
            tileData.contains = null;
            _tileDataMap[position] = tileData;
        }
    }

    public Dictionary<Directions.Direction, Vector3Int> GetAdjacentTiles(Vector3Int tilePosition)
    {
        return new Dictionary<Directions.Direction, Vector3Int>
        {
            { Directions.Direction.Up, new Vector3Int(tilePosition.x, tilePosition.y + 1, tilePosition.z) },
            { Directions.Direction.Down, new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z) },
            { Directions.Direction.Left, new Vector3Int(tilePosition.x - 1, tilePosition.y, tilePosition.z) },
            { Directions.Direction.Right, new Vector3Int(tilePosition.x + 1, tilePosition.y, tilePosition.z) }
        };
    }

    public Vector3 GetWorldPosition(Vector3Int tilePosition)
    {
        return parentGrid.CellToWorld(tilePosition) + parentGrid.cellSize / 2;
    }

    public Vector3Int GetRandomPathableTile()
    {
        return _walkableTiles[Random.Range(0, _walkableTiles.Count)].position;
    }

    public bool IsWalkable(Vector3Int position)
    {
        if (_tileDataMap.ContainsKey(position))
        {
            TileData tileData = _tileDataMap[position];
            return tileData.tileType == pathableTile;
        }
        
        TileBase tile = GetTile(position);
        return tile != null && tile == pathableTile;
    }

    public void MoveEntity(GameObject entity, Directions.Direction direction)
    {
        IEntity entityComponent = entity.GetComponent<IEntity>();
        Transform entityTransform = entity.GetComponent<Transform>();

        Vector3Int dirVector = Directions.GetDirectionVector(direction);

        Vector3Int currentPosition = entityComponent.CurrentTile;
        Vector3Int newTilePosition = currentPosition + dirVector;
        TileBase targetTile = GetTile(newTilePosition);

        if (targetTile != null && !HasEntity(newTilePosition) && targetTile == pathableTile)
        {
            RemoveEntity(currentPosition);
            entityComponent.CurrentTile = newTilePosition;
            SetEntity(newTilePosition, entityComponent);
            entityTransform.position = GetWorldPosition(newTilePosition);
        }
    }
}
