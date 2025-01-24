using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CompeteStrategy : StrategyBase
{


    public PlayerData targetOpponent;
    private List<Node> targetTiles;


    private Vector2Int lastGridPosition;
    private float stuckTime;
    private const float maxStuckTime = 3f;
    private bool isInitialized = false;
    private const int minOwnedTilesToCompete = 10;

    public override void Enter()
    {
        EvaluateAvailability();
        targetTiles = new List<Node>();
        lastGridPosition = aiController.GetPlayerData().CurrentGridPosition;

    }

    public override void Execute()
    {
        // Initialize tracking variables during the first execution
        if (!isInitialized)
        {
            lastGridPosition = aiController.GetPlayerData().CurrentGridPosition;
            stuckTime = 0f;
            isInitialized = true;
        }

        // Stuck detection logic
        Vector2Int currentGridPosition = aiController.GetPlayerData().CurrentGridPosition;
        if (currentGridPosition == lastGridPosition)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > maxStuckTime)
            {

                sam.SetStrategyAvailability("CompeteStrategy", false);
                ExitAndSwitchToAvailableStrategy();
                return;
            }
        }
        else
        {
            stuckTime = 0f; // Reset if the AI moves
        }
        lastGridPosition = currentGridPosition;

        // Existing Execute logic
        if (aiController.GetPlayerData().OwnedTiles.Count <= minOwnedTilesToCompete)
        {
            sam.SetStrategyAvailability("CompeteStrategy", false);
            Exit();
            return;
        }

        if (targetOpponent == null || targetTiles.Count == 0)
        {
            targetOpponent = SelectOpponentWithHighestScore();
            if (targetOpponent != null)
            {
                SetTargetTiles(targetOpponent);
             //   aiController.GetCognitionSystem().AdjustEyesForCompeteStrategy(targetOpponent);  
               
            }
        }

        if (targetTiles.Count > 0)
        {
            Node targetNode = targetTiles[0];
            aiController.SetTargetNode(targetNode);

            if (aiController.ReachedTarget())
            {
                aiController.GetPlayerData().AddOwnedTile(targetNode);
                targetTiles.RemoveAt(0);

                if (targetTiles.Count == 0)
                {
                    sam.SetStrategyAvailability("CompeteStrategy", false);
                    ExitAndSwitchToAvailableStrategy();
                }
            }
        }
        else
        {
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
        List<(PlayerData, int)> opponents = aiController.GetPlayerData().OpponentsWithUI;
        PlayerData highestScoringOpponent = null;
        int maxScore = 0;

        foreach (var (opponent, _) in opponents) // Deconstruct the tuple to get the PlayerData
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









