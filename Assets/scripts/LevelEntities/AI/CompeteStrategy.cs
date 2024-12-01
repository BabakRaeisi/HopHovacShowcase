using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CompeteStrategy : StrategyBase
{
   
   
    private PlayerData targetOpponent;
    private List<Node> targetTiles;

    private const int minOwnedTilesToCompete = 10;
     
    public override void Enter()
    {
        EvaluateAvailability();
        targetTiles = new List<Node>();
    }

    public override void Execute()
    {
        // Check if the AI still meets the criteria to compete
        if (aiController.GetPlayerData().OwnedTiles.Count <= minOwnedTilesToCompete)
        {
          //  Debug.Log("CompeteStrategy: Not enough tiles owned, exiting strategy.");
            sam.SetStrategyAvailability("CompeteStrategy", false);
            Exit();
            return;
        }

        // If no target opponent is set, find one
        if (targetOpponent == null || targetTiles.Count == 0)
        {
            targetOpponent = SelectOpponentWithHighestScore();
            if (targetOpponent != null)
            {
                SetTargetTiles(targetOpponent);
            }
        }

        // If we have valid target tiles, continue capturing
        if (targetTiles.Count > 0)
        {
            Node targetNode = targetTiles[0];
            aiController.SetTargetNode(targetNode);

            // If the AI reaches the target, capture the tile and move to the next
            if (aiController.ReachedTarget())
            {
                aiController.GetPlayerData().AddOwnedTile(targetNode);
                targetTiles.RemoveAt(0); // Move to the next tile

                // Check if we have captured enough tiles
                if (targetTiles.Count == 0)
                {
                  //  Debug.Log("CompeteStrategy: Captured required tiles, exiting strategy.");
                    sam.SetStrategyAvailability("CompeteStrategy", false);
                    ExitAndSwitchToAvailableStrategy();
                }
            }
        }
        else
        {
            // If no more tiles to capture, exit and switch
           // Debug.Log("CompeteStrategy: No target tiles left, exiting strategy.");
            sam.SetStrategyAvailability("CompeteStrategy", false);
            ExitAndSwitchToAvailableStrategy();
        }
    }


    public override void Exit()
    {
        targetOpponent = null;
        targetTiles.Clear();
    }

    public override void EvaluateAvailability()
    {
        bool available = aiController.GetPlayerData().OwnedTiles.Count > minOwnedTilesToCompete;
        sam.SetStrategyAvailability("CompeteStrategy", available);
      /*  Debug.Log($"CompeteStrategy availability: {available}");*/
    }

    private void SetTargetTiles(PlayerData opponent)
    {
        if (opponent.OwnedTiles.Count <= 3)
        {
          /*  Debug.Log("CompeteStrategy: Opponent has too few tiles, exiting strategy.");*/
            sam.SetStrategyAvailability("CompeteStrategy", false);
            Exit();
            return;
        }

        // Capture half of the opponent's tiles
        int numTilesToCapture = Mathf.CeilToInt(opponent.OwnedTiles.Count / 2.0f);
        targetTiles = opponent.OwnedTiles.GetRange(0, numTilesToCapture);

        if (targetTiles.Count == 0)
        {
      /*      Debug.Log("CompeteStrategy: No tiles to target, exiting strategy.");*/
            sam.SetStrategyAvailability("CompeteStrategy", false);
            Exit();
        }
    }
    private void ExitAndSwitchToAvailableStrategy()
    {
        Exit();
        aiController.GetCognitionSystem().EvaluateAndSetActiveStrategy();
    }

    private PlayerData SelectOpponentWithHighestScore()
    {
        List<PlayerData> opponents = aiController.GetPlayerData().Opponents;
        PlayerData highestScoringOpponent = null;
        int maxScore = 0;

        foreach (var opponent in opponents)
        {
            if (opponent.Points > maxScore)
            {
                maxScore = opponent.Points;
                highestScoringOpponent = opponent;
            }
        }
        return highestScoringOpponent;
    }
  
}









