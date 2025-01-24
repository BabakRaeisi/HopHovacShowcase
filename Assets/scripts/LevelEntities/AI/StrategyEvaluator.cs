
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrategyEvaluator
{
    private StrategyAvailabilityManager sam;
    private AIController aiController;
    private PlayerData playerData;
    private AICognition aiCognition;

    public StrategyEvaluator(StrategyAvailabilityManager sam, AIController aiController, PlayerData playerData, AICognition aiCognition)
    {
        this.sam = sam;
        this.aiController = aiController;
        this.playerData = playerData;
        this.aiCognition = aiCognition;
    }

    public StrategyBase EvaluateBestStrategy()
    {

        // Priority Level 2: Evaluate Collectable and Compete Strategies
        Dictionary<StrategyBase, float> strategyScores = new Dictionary<StrategyBase, float>();

        if (sam.IsStrategyAvailable("CollectableStrategy"))
        {
            float collectableScore = EvaluateCollectableStrategy();
            strategyScores.Add(aiCognition.GetStrategyByType<CollectableStrategy>(), collectableScore);
        }

        if (sam.IsStrategyAvailable("CompeteStrategy"))
        {
            float competeScore = EvaluateCompeteStrategy();
            strategyScores.Add(aiCognition.GetStrategyByType<CompeteStrategy>(), competeScore);
        }

        // Choose the best strategy among the Level 2 strategies
        if (strategyScores.Count > 0)
        {
            return strategyScores.OrderByDescending(pair => pair.Value).First().Key;
        }

        // Priority Level 3: Explore Strategy (fallback)
        if (sam.IsStrategyAvailable("ExploreStrategy"))
        {
            return aiCognition.GetStrategyByType<ExploreStrategy>();
        }

        return null;
    }



    private float EvaluateCollectableStrategy()
    {
        float score = 0;
        List<Collectable> activeCollectables = aiController.GetActiveCollectables();

        foreach (var collectable in activeCollectables)
        {
            float distance = Vector2Int.Distance(playerData.CurrentGridPosition, collectable.AssignedNode.Coordinates);
            if (distance <= 3)
            {
                score += 20;
            }
        }

        if (playerData.Points < GetHighestOpponentPoints())
        {
            score += 15;
        }
        return score;
    }

    private float EvaluateCompeteStrategy()
    {
        float score = 0;
        if (playerData.OwnedTiles.Count > 10)
        {
            score += 10;
            if (playerData.Points < GetHighestOpponentPoints())
            {
                score += 20;
            }
        }
        return score;
    }

    private int GetHighestOpponentPoints()
    {
        if (playerData.OpponentsWithUI.Count == 0)
        {
            return 0;
        }

        // Extract the highest points from the OpponentsWithUI list
        return playerData.OpponentsWithUI.Max(opponent => opponent.Item1.Points);
    }


    private PlayerData GetNearestOpponent()
    {
        return playerData.OpponentsWithUI
            .OrderBy(opponent => Vector2Int.Distance(playerData.CurrentGridPosition, opponent.Item1.CurrentGridPosition))
            .Select(opponent => opponent.Item1) // Extract the PlayerData
            .FirstOrDefault();
    }
}

