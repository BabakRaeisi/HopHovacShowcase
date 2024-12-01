using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerData :MonoBehaviour {


    [Header("VFX")]
    public GameObject DisabledVFX; 
    public GameObject HitVFX;
    

    // Basic player properties
    [SerializeField] private string playerName;              // Name of the player
     
    [SerializeField] private float speed = 5f;               // Speed of the player
    [SerializeField] private Color playerColor = Color.white; // Color associated with the player
    [SerializeField] private int points = 0;                 // Player's current points
    [SerializeField] private float speedRotation = 5f;       // Speed at which the player rotates
    [SerializeField] private Vector2Int currentGridPosition; // Player's position on the grid
    [SerializeField] private Node targetNode;
    [SerializeField] private GridSystem gridSystem;
    
    // Opponent tracking properties

    private List<PlayerData> opponents = new List<PlayerData>();  // Private list of opponents
    public PlayerData MainCompetitor { get; private set; }        // Primary competitor for score or position
    public Vector2Int TargetLocation;     
    public List<Vector2Int>Path = new List<Vector2Int>();

    public ProjectileCollectable ActiveCollectable;
    private void Awake()
    {
            gridSystem = FindAnyObjectByType<GridSystem>();
            gridSystem.playersList.Add(this);
    }

    public Node TargetNode { get { return targetNode; } set
        { 
            targetNode = value;
            TargetLocation = targetNode.Coordinates;
        }
    }

    // Tracking tile ownership and collectables
    private List<Node> ownedTiles = new List<Node>();  // List of nodes owned by the player
    public bool hasMissile = false;                   // Flag to track missile possession



    // Properties
    public List<PlayerData> Opponents => opponents;    // Public getter for Opponents
    public List<Node> OwnedTiles => ownedTiles;        // Public getter for Owned Tiles

    // Accessors for properties
    public string PlayerName => playerName;
    public float Speed => speed;
    public Color PlayerColor => playerColor;
    public int Points => points;
    public float SpeedRotation => speedRotation;
    public Vector2Int CurrentGridPosition
    {
        get => currentGridPosition;
        set => currentGridPosition = value;
    }

    // Methods to manage points
    public void AddPoints()
    {
        // Add points based on the number of owned tiles, excluding the last one
        if (ownedTiles.Count > 1)
            points += ownedTiles.Count - 1;
        else
            points += ownedTiles.Count;

        // If there's only one tile, skip resetting
        if (ownedTiles.Count == 0) return;

        // Create a copy of the owned tiles list, excluding the last tile
        Node lastNode = ownedTiles[ownedTiles.Count - 1];
        List<Node> nodesToReset = ownedTiles.GetRange(0, ownedTiles.Count - 1);

        // Reset ownership for each node, except the last one
        foreach (Node node in nodesToReset)
        {
            node.ResetOwnership();
        }

        // Clear all nodes except the last one
        ownedTiles.Clear();
        ownedTiles.Add(lastNode);
    }

    public void ResetPoints()
    {
        points = 0;
    }

    public void HitDisable() 
    {
    HitVFX.SetActive(true);
    StartCoroutine(Disable());
    }
    private IEnumerator Disable()
    {
        DisabledVFX.SetActive(true);
        speed = 0;
        yield return new WaitForSeconds(3.5f);
        DisabledVFX.SetActive(false);
        HitVFX.SetActive(false);
        speed = 5;
    
    }
    // Opponent management
    public void SetOpponents(List<PlayerData> newOpponents)
    {
        
        opponents = newOpponents;
       
    }

    public void SetMainCompetitor(PlayerData competitor)
    {
        MainCompetitor = competitor;
    }

    // Owned tile management
    public void AddOwnedTile(Node tile)
    {
        if (!ownedTiles.Contains(tile))
            ownedTiles.Add(tile);
    }

    public void RemoveOwnedTile(Node tile)
    {
        ownedTiles.Remove(tile);
    }

    // Missile management - player can only have one missile at a time
    public bool HasMissile => hasMissile;

    public bool AddMissile()
    {
        if (!hasMissile)
        {
            hasMissile = true;
            return true; // Missile added successfully
        }
        return false; // Player already has a missile, no additional missile added
    }

    public void UseMissile()
    {
        if (hasMissile)
            hasMissile = false; // Missile is used up and removed
    }
}
