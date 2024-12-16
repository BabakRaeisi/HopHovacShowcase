
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
        // Priority Level 1: Missile Strategy
        if (sam.IsStrategyAvailable("ProjectileStrategy"))
        {
            return aiCognition.GetStrategyByType<ProjectileStrategy>();
        }

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

    private float EvaluateMissileStrategy()
    {
        if (playerData.HasAbility)
        {
            float score = 100; // Highest priority when a missile is available
            PlayerData nearestOpponent = GetNearestOpponent();
            if (nearestOpponent != null)
            {
                float distance = Vector2Int.Distance(playerData.CurrentGridPosition, nearestOpponent.CurrentGridPosition);
                if (distance < 3)
                {
                    score += 20;
                }
            }
            return score;
        }
        return 0;
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
        return playerData.Opponents.Max(opponent => opponent.Points);
    }

    private PlayerData GetNearestOpponent()
    {
        return playerData.Opponents.OrderBy(opponent => Vector2Int.Distance(playerData.CurrentGridPosition, opponent.CurrentGridPosition)).FirstOrDefault();
    }
}

