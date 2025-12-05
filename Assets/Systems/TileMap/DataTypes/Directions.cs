using UnityEngine;

public class Directions
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public static Vector3Int GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector3Int(0, 1, 0),
            Direction.Down => new Vector3Int(0, -1, 0),
            Direction.Left => new Vector3Int(-1, 0, 0),
            Direction.Right => new Vector3Int(1, 0, 0),
            _ => Vector3Int.zero
        };
    }
}
