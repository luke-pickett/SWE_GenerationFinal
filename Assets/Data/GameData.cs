using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data")]
public class GameData : ScriptableObject
{
    [SerializeField] private int highScore = 0;

    public int HighScore
    {
        get { return highScore; }
        set
        {
            if (value > highScore)
            {
                highScore = value;
                Debug.Log($"New high score: {highScore}");
            }
        }
    }

    public void ResetHighScore()
    {
        highScore = 0;
    }
}
