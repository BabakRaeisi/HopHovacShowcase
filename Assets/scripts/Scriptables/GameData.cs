using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data")]
public class GameData : ScriptableObject
{
    [Header("User Selection")]
    public int selectedCharacterIndex; // The index of the user-selected character

    [Header("All Available Prefabs and Sprites")]
    public GameObject[] userCharacterPrefabs; // All prefabs for user-controlled characters
    public GameObject[] aiCharacterPrefabs; // All prefabs for AI-controlled characters
    public Sprite[] characterSprites; // All character sprites (shared for user and AI)

    [Header("Grid Positions")]
    public Vector2Int userStartPosition = new Vector2Int(0, 0); // Default start position for the user
    public Vector2Int[] aiStartPositions = new Vector2Int[]
    {
        new Vector2Int(0, 7), new Vector2Int(7, 0), new Vector2Int(7, 7) // Default AI positions
    };

    [Header("Gameplay Settings")]
    public int initialScore = 0; // Default starting score for all characters
}
