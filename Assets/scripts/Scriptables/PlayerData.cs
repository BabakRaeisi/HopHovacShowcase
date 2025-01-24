using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
 
using UnityEngine;
 


public class PlayerData :MonoBehaviour {


    public PlayerUIInfo playerUIInfo;

    public PlayerState state = PlayerState.Neutral;
    private float speed = 5f;               
    private int points = 0;                 
    private float speedRotation = 5f;       
    private Vector2Int currentGridPosition;
    private Vector2Int initialGridPosition;
    private Node targetNode;
    private  GridSystem gridSystem;
    [SerializeField]private AbilityPoolManager abilityPoolManager;
    private Ability currentAbility;
    private List<(PlayerData, int)> opponentsWithUI;
    private List<Node> ownedTiles = new List<Node>();  // List of nodes owned by the player
    private bool hasAbility = false;
    private bool hasShield = false;
    
    // Private list of opponents
    public PlayerData MainCompetitor { get; private set; }        // Primary competitor for score or position
 
    public event Action OnReset;
    
    // Properties

    public List<(PlayerData, int)> OpponentsWithUI
    {
        get => opponentsWithUI;
        set => opponentsWithUI = value; // Allow external assignment
    }   // Public getter for Opponents
    public List<Node> OwnedTiles => ownedTiles;        // Public getter for Owned Tiles

    public bool HasAbility { get { return hasAbility; } set { hasAbility = value; } }
    public bool HasShield { get { return hasShield; } set { hasShield = value; } }
    public int  ImageIndex { get; set; }
    public float Speed => speed;
    public Material PlayerMaterial  ;
    public int Points => points;
    public float SpeedRotation => speedRotation;
    public Vector2Int CurrentGridPosition
    {
        get => currentGridPosition;
        set => currentGridPosition = value;
    }
    public Vector2Int InitialGridPosition
    {
        get => initialGridPosition;
        set => initialGridPosition = value;
    }


    public Vector3 DirectionVec;
     
    public bool isDuringRound;

    [Header("VFX")]
    public GameObject DisabledVFX;
   
    public GameObject HasAbilityVFX;
    public GameObject HasShieldVFX;

   
    public PlayerState State
    {
        get => state;
        set => state = value;
    }

    private void Awake()
    {
            gridSystem = FindAnyObjectByType<GridSystem>();
            abilityPoolManager = FindAnyObjectByType<AbilityPoolManager>();
            gridSystem.playersList.Add(this);
    }

    public Node TargetNode { get { return targetNode; } set
        { 
            targetNode = value;
        }
    }

    public void ResetPlayer() 
    {
        state = PlayerState.Neutral; // Reset to neutral


        this.transform.position = gridSystem.GetWorldPositionFromGrid(initialGridPosition);
        currentGridPosition = initialGridPosition;
       
        ResetPoints();
        RemoveAbility(currentAbility);
       
        DisableVFX(DisabledVFX);
        
        DisableVFX(HasAbilityVFX);
        DisableVFX(HasShieldVFX);
        speed = 5;
        hasAbility = false;
        hasShield = false;
        ownedTiles.Clear();
        gridSystem.PlayerManager.TryMovePlayer(this, currentGridPosition);
        OnReset?.Invoke();
    }
   

    public Vector3 GetFrontGridPosition()
    {
        Vector2Int direction = new Vector2Int(
            Mathf.RoundToInt(DirectionVec.x),
            Mathf.RoundToInt(DirectionVec.z)
        );
        return gridSystem.GetWorldPositionFromGrid(CurrentGridPosition + direction) + Vector3.up * 0.5f;
    }

    public void SetShield(bool state) 
    {  
        HasShield = state; 
        EnableVFX(HasShieldVFX);
        StartCoroutine(ShieldDuration());
        SoundManager.Instance.PlayOneShotSound("Shield", this.transform.position);
    }

    private IEnumerator ShieldDuration()
    {
         yield return new WaitForSeconds(5.5f);
        HasShield=false;
        DisableVFX(HasShieldVFX);
    }

 
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
        playerUIInfo.playerScore.text = points.ToString();
    }
   

    public void ResetPoints()
    {
        points = 0;
        playerUIInfo.playerScore.text = points.ToString();
    }

 
    // Opponent management
    public void SetOpponents(List<(PlayerData,int)> newOpponents)
    {
        opponentsWithUI = newOpponents;
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
    
     

    public void AddAbility(AbilityType abilityType)
    {

        if (!hasAbility&&state==PlayerState.Active)
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

        if (currentAbility == null)
        {
            Debug.LogError($"[PlayerData] Failed to get ability of type {abilityType}. Pool might be empty or not initialized.");
            return;
        }

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
            currentAbility.Activate();
            hasAbility = false;
            DisableVFX(HasAbilityVFX);
        }
    }
    public void RemoveAbility(Ability currentAbility)
    {
        if (currentAbility != null)
        {
           
            hasAbility = false;

            currentAbility.transform.SetParent(abilityPoolManager.transform);
            abilityPoolManager.ReturnAbility(currentAbility, currentAbility.AbilityType);

            currentAbility = null;
            DisableVFX(HasAbilityVFX);
        }
    }

    public void SetRoundStatus(bool state) => isDuringRound = state;
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
    private Vector3 offsetVFX = new Vector3(0, 2f, 0);
    public void HitDisable(EffectType VFX) // get proper VFX with enum
    {
        if (state == PlayerState.Disabled) return;
        if (state == PlayerState.Active)
        {
            if (hasShield)
            {

                StartCoroutine(ShieldDisable());

                return;
            }
            state = PlayerState.Disabled;
            RemoveAbility(currentAbility);
            
            abilityPoolManager.GetEffect(VFX, this.transform.position + offsetVFX);
            
            
            // Remove ability if present

            // Start the speed reduction coroutine and snap to the grid at the end
            StartCoroutine(DisableWithSpeedReduction());
        }
    }
    private IEnumerator ShieldDisable()
    {
        yield return new WaitForSeconds(1.5f);
        hasShield = false;
        SoundManager.Instance.PlayOneShotSound("Shield", this.transform.position);
        DisableVFX(HasShieldVFX);
    }
    private IEnumerator DisableWithSpeedReduction()
    {
        // Smoothly reduce speed to zero
        yield return StartCoroutine(SmoothSpeedReduction(1f)); // Adjust duration as needed

        // Snap the player to the nearest grid position
       StartCoroutine(SmoothSnapToGridPosition(1f));
        SoundManager.Instance.PlayOneShotSound("Dizzy", this.transform.position);
        // Wait for the rest of the disable duration
        DisabledVFX.SetActive(true);
        yield return new WaitForSeconds(2.5f); // Adjust disable time as needed

        // Re-enable speed and disable VFX
        DisableVFX(DisabledVFX);
       
        if (isDuringRound)
        {
            state = PlayerState.Active;
            speed = 5;
        }
       
    }
}
public enum PlayerState
{
    Neutral,    // Before or after the round (reset state)
    Active,     // During the round, player can move
    Disabled,   // Temporarily unable to act (e.g., hit or stunned)
    Locked      // Locked during setup or between rounds
}