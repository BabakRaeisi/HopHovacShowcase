using System.Collections.Generic;
using UnityEngine;

public class CollectableStrategy : StrategyBase
{
    private Collectable currentTargetCollectable;
    private const float maxCollectableDistance = 3f;
    public Color Called;


    public override void Initialize(StrategyAvailabilityManager sam, PlayerData playerData, AIController aiController)
    {
        base.Initialize(sam, playerData, aiController);
        CollectablePoolManager.OnCollectableSpawned += OnCollectableSpawned;
        CollectablePoolManager.OnCollectableDespawned += OnCollectableDespawned;
        
    }
    public override void Enter()
    {
        EvaluateAvailability();
    }

    public override void Execute()
    {
        if (currentTargetCollectable == null || !currentTargetCollectable.gameObject.activeInHierarchy)
        {
            SetNewTargetCollectable();
            return;
        }

        float distanceToCollectable = Vector2Int.Distance(
            aiController.GetPlayerData().CurrentGridPosition,
            currentTargetCollectable.AssignedNode.Coordinates
        );

        if (distanceToCollectable > maxCollectableDistance)
        {
            currentTargetCollectable = null;
            ExitStrategy();
            return;
        }

        if (aiController.ReachedTarget() && currentTargetCollectable != null)
        {
            aiController.GetPlayerData().AddOwnedTile(aiController.GetPlayerData().TargetNode);
            currentTargetCollectable.Collect(aiController.GetPlayerData());

            currentTargetCollectable = null;

            // Instead of resetting cooldown, try to collect the next item
            SetNewTargetCollectable();
        }
    }


    public override void Exit()
    {
        currentTargetCollectable = null;
    }

    public override void EvaluateAvailability()
    {
        if (aiController == null) return;

        List<Collectable> activeCollectables = aiController.GetActiveCollectables();
        PlayerData playerData = aiController.GetPlayerData();

        if (activeCollectables == null || activeCollectables.Count == 0)
        {
            NotifyAvailability("CollectableStrategy", false);
            return;
        }

        bool isAvailable = false;

        foreach (var collectable in activeCollectables)
        {
            float distance = Vector2Int.Distance(playerData.CurrentGridPosition, collectable.AssignedNode.Coordinates);
            if (distance <= maxCollectableDistance)
            {
                isAvailable = true;
                break;
            }
        }

        NotifyAvailability("CollectableStrategy", isAvailable);
    }

    private void OnCollectableSpawned(Collectable collectable)
    {
        Called = Color.green;
        EvaluateAvailability();
    }

    private void OnCollectableDespawned(Collectable collectable)
    {
        Called = Color.red;
        if (collectable == currentTargetCollectable)
        {
            currentTargetCollectable = null;
            NotifyAvailability("CollectableStrategy", false);
            ExitStrategy();
        }
    }

    private void SetNewTargetCollectable()
    {
        List<Collectable> activeCollectables = aiController.GetActiveCollectables();
        if (activeCollectables == null || activeCollectables.Count == 0) return;

        PlayerData playerData = aiController.GetPlayerData();
        Collectable closestCollectable = FindClosestCollectable(activeCollectables, playerData.CurrentGridPosition);

        if (closestCollectable != null)
        {
            currentTargetCollectable = closestCollectable;
            aiController.SetTargetNode(currentTargetCollectable.AssignedNode);
        }
    }

    private Collectable FindClosestCollectable(List<Collectable> activeCollectables, Vector2Int currentPos)
    {
        Collectable closestCollectable = null;
        float minDistance = float.MaxValue;

        foreach (var collectable in activeCollectables)
        {
            float distance = Vector2Int.Distance(currentPos, collectable.AssignedNode.Coordinates);
            if (distance < minDistance && distance <= maxCollectableDistance)
            {
                minDistance = distance;
                closestCollectable = collectable;
            }
        }

        return closestCollectable;
    }

    private void ExitStrategy()
    {
        Exit();
        aiController.GetCognitionSystem().EvaluateAndSetActiveStrategy();
    }
}
