using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GridSystem : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;
    public int UnityGridSize { get { return unityGridSize; } }
    public PlayerManager PlayerManager { get { return playerManager; } }
    public Vector2Int GridSize { get { return gridSize; } }

    public List<PlayerData>playersList ;

    public Dictionary<Vector2Int, Node> Grid
    {
        get { return grid; }
    }

    [SerializeField] List<GameObject> tileGameObjects;  // Manually assign these in the editor
    [SerializeField] Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();  // Grid dictionary to hold nodes
   
    [SerializeField] private Dictionary<PlayerData, Node> playerCurrentNodes = new Dictionary<PlayerData, Node>();  // Dictionary to hold players and their current nodes

    private GridUtility gridInitializer;
    private PlayerManager playerManager;
    #region InitializeGrid
    void Awake()
    {
        gridInitializer = new GridUtility();
        grid = gridInitializer.InitializeGrid(tileGameObjects, unityGridSize);


        playerManager = new PlayerManager(this);
        playerManager.UpdateOpponents(playersList);

    }
   

    #endregion

    // Converts world position to grid coordinates
    public Vector2Int GetTileCoordinates(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / unityGridSize);
        int y = Mathf.RoundToInt(worldPosition.z / unityGridSize);
        return new Vector2Int(x, y);
    }
    public Node GetNodeAtPosition(Vector2Int position)
    {
        if (grid.TryGetValue(position, out Node node))
        {
            return node;  // Return the node if it exists at the given position
        }
        else
        {
            Debug.LogWarning($"No node found at position: {position}");
            return null;  // Return null if no node is found at the given position
        }
    }

    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPosition)
    {
        // Convert grid coordinates to world position
        float worldX = gridPosition.x * unityGridSize;
        float worldZ = gridPosition.y * unityGridSize;

        return new Vector3(worldX, 0, worldZ);  // Return the world position, assuming ground is at y = 0
    }


    // Check if the target position is within grid boundaries and the node is not occupied
    public bool IsValidPosition(Vector2Int newCoords)
    {
        return grid.ContainsKey(newCoords) && !grid[newCoords].IsOccupied;
    }


    public Node GetRandomUnoccupiedNode(PlayerData playerData = null)
    {
        List<Vector2Int> keys = new List<Vector2Int>(grid.Keys);  // Get all keys from the dictionary
        int maxAttempts = 100;  // Prevent infinite loop in case most nodes are occupied
        int attempts = 0;

        // Randomly select nodes until we find an unoccupied one or reach max attempts
        while (attempts < maxAttempts)
        {
            Vector2Int randomKey = keys[Random.Range(0, keys.Count)];  // Pick a random key
            Node randomNode = grid[randomKey];  // Get the node associated with the random key

            // Check if the node is unoccupied and doesn't have a collectable
            if (!randomNode.IsOccupied && (randomNode.Owner == null|| !randomNode.Owner==playerData ))
            {
                return randomNode;  // Found a valid node
            }

            attempts++;  // Increase attempt count to prevent infinite looping
        }

        Debug.LogWarning("No unoccupied node found after maximum attempts.");
        return null;  // No unoccupied nodes found after max attempts
    }

    // Example method to return player's starting position (can be hardcoded or dynamic)
    public Vector2Int GetPlayerStartingPosition(Transform player)
    {
         
        return  GetTileCoordinates(player.transform.position);  // Use the actual position from the scene
    }
}
