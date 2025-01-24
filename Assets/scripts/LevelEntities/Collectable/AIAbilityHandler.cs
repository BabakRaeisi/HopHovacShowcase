using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAbilityHandler : MonoBehaviour
{
    private PlayerData playerData;
    private float maxHoldTime = 5f; // Max time to hold the projectile
    private float currentHoldTime;

    private bool isAligningAndShooting; // Track whether alignment/shooting is in progress
    private bool hasTargetInAlignment;

    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
    }

    private void OnEnable()
    {
        ResetActivation();
    }

    private void Update()
    {
        
        if (!playerData.HasAbility || isAligningAndShooting) return;

        // Check alignment with opponents
        hasTargetInAlignment = CheckAlignment();
   
        if (hasTargetInAlignment)
        {
            
            // Start alignment and shoot process without delay
            AlignAndShoot();
        }
        else
        {
            // Countdown to auto-shoot if no alignment
            currentHoldTime -= Time.deltaTime;
            if (currentHoldTime <= 0)
            {
                AutoShoot();
            }
        }
    }

    private bool CheckAlignment()
    {
        foreach (var (opponent, _) in playerData.OpponentsWithUI) // Iterate over the OpponentsWithUI list
        {
            // Calculate direction to opponent
            Vector3 directionToOpponent = (opponent.transform.position - transform.position).normalized;

            // Use DirectionVec to enforce cardinal directions
            Vector3 cardinalDirection = new Vector3(
                Mathf.RoundToInt(playerData.DirectionVec.x),
                0,
                Mathf.RoundToInt(playerData.DirectionVec.z)
            ).normalized;

            if (Vector3.Dot(cardinalDirection, directionToOpponent) > 0.95f) // Alignment threshold
            {
                return true; // Found an aligned opponent
            }
        }
        return false; // No opponents in alignment
    }

    private void AlignAndShoot()
    {
        if (!(playerData.state == PlayerState.Active)) return;
        isAligningAndShooting = true;

        // Ensure alignment is still valid
        if (hasTargetInAlignment)
        {
             playerData.UseAbility();  
        }

        ResetActivation();
        isAligningAndShooting = false;
    }

    private void AutoShoot()
    {
        if (!(playerData.state==PlayerState.Active)) return;
        playerData.UseAbility(); // Launch the projectile after timeout
        ResetActivation();
    }

    private void ResetActivation()
    {
        currentHoldTime = maxHoldTime;
        hasTargetInAlignment = false;
    }
}
