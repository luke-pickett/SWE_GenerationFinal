using UnityEngine;

public interface IEntity
{
    Vector3Int CurrentTile { get; set; }
    Directions.Direction FacingDirection { get; set; }
}
