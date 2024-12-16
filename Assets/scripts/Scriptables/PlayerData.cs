using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerData :MonoBehaviour {


    [Header("VFX")]
    public GameObject DisabledVFX; 
    public GameObject HitVFX;
    public GameObject HasAbilityVFX;

    [Header("player settings")]
    // Basic player properties
    [SerializeField] private string playerName;              // Name of the player
    [SerializeField] private float speed = 5f;               // Speed of the player
    [SerializeField] private Color playerColor = Color.white; // Color associated with the player
    [SerializeField] private int points = 0;                 // Player's current points
    [SerializeField] private float speedRotation = 5f;       // Speed at which the player rotates
    [SerializeField] private Vector2Int currentGridPosition;  
    [SerializeField] private Node targetNode;
      public  GridSystem gridSystem;
    
    
    // Opponent tracking properties

    private List<PlayerData> opponents = new List<PlayerData>();  // Private list of opponents
    public PlayerData MainCompetitor { get; private set; }        // Primary competitor for score or position
    public Vector2Int TargetLocation;     
     
    public AbilityPoolManager abilityPoolManager;

    private void Awake()
    {
            gridSystem = FindAnyObjectByType<GridSystem>();
            abilityPoolManager = FindAnyObjectByType<AbilityPoolManager>();
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
    public bool hasAbility = false;                   // Flag to track missile possession



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
     
    public GameObject projectileObject;
    public Vector3 DirectionVec;

    public Ability currentAbility;
    public bool Disabled; 

  
    private Vector3 GetFrontGridPosition()
    {
        Vector2Int direction = new Vector2Int(
            Mathf.RoundToInt(DirectionVec.x),
            Mathf.RoundToInt(DirectionVec.z)
        );
        return gridSystem.GetWorldPositionFromGrid(CurrentGridPosition + direction) + Vector3.up * 0.5f;
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
    
     
    public bool HasAbility => hasAbility;
    public void AddAbility(AbilityType abilityType)
    {

        if (!hasAbility)
        {
            GetAbility(abilityType);
        }
        else
        {
            RemoveAbility(currentAbility);
            GetAbility(abilityType);
        }
    }

    private void GetAbility(AbilityType abilityType)
    {
        currentAbility = abilityPoolManager.GetAbility(abilityType, this);
        currentAbility.transform.SetParent(this.transform);


        currentAbility.transform.position = GetFrontGridPosition();


        currentAbility.gameObject.SetActive(false);


        hasAbility = true;
        EnableVFX(HasAbilityVFX);
    }

    public void UseAbility()
    {
        if (hasAbility && currentAbility != null)
        {
            currentAbility.gameObject.SetActive(true);
            currentAbility.Activate(DirectionVec, GetFrontGridPosition());
            hasAbility = false;
            DisableVFX(HasAbilityVFX);
        }
    }
    public void RemoveAbility(Ability currentAbility)
    {
        if (currentAbility != null)
        {
            // Return the ability to the AbilityPoolManager
            currentAbility.transform.SetParent(abilityPoolManager.transform);

            // Deactivate and return the ability to the pool
            abilityPoolManager.ReturnAbility(currentAbility, currentAbility.AbilityType);

            currentAbility = null;
            hasAbility = false;
            DisableVFX(HasAbilityVFX);
        }
    }

    private void DisableVFX(GameObject gameObject) 
    {
    gameObject.SetActive(false);
    
    }
    private void EnableVFX(GameObject gameObject) 
    {

        gameObject.SetActive(true);
    }

    private IEnumerator SmoothSpeedReduction(float duration)
    {
        float initialSpeed = speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Gradually reduce speed to zero
            speed = Mathf.Lerp(initialSpeed, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure speed is exactly zero at the end
        speed = 0;
    }

    private IEnumerator SmoothSnapToGridPosition(float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = gridSystem.GetWorldPositionFromGrid(currentGridPosition);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Smoothly interpolate the position
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player is exactly at the target position
        transform.position = targetPosition;
    }

    public void HitDisable()
    {
        EnableVFX(HitVFX);
        Disabled = true;
        // Remove ability if present
        if (hasAbility)
        {
            RemoveAbility(currentAbility);
        }

        // Start the speed reduction coroutine and snap to the grid at the end
        StartCoroutine(DisableWithSpeedReduction());
    }

    private IEnumerator DisableWithSpeedReduction()
    {
        // Smoothly reduce speed to zero
        yield return StartCoroutine(SmoothSpeedReduction(1f)); // Adjust duration as needed

        // Snap the player to the nearest grid position
       StartCoroutine(SmoothSnapToGridPosition(1f));

        // Wait for the rest of the disable duration
        DisabledVFX.SetActive(true);
        yield return new WaitForSeconds(2.5f); // Adjust disable time as needed

        // Re-enable speed and disable VFX
        DisableVFX(DisabledVFX);
        DisableVFX(HitVFX);
        Disabled = false;
        speed = 5;
    }
}
