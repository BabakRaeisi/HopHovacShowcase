using UnityEngine;

public class ProjectileStrategy : StrategyBase
{
    private PlayerData targetPlayer; // Target to attack
    private bool isAligned = false;

    public override void Enter()
    {
        EvaluateAvailability();
        SelectTarget();
    }

    public override void EvaluateAvailability()
    {
        bool hasMissile = aiController.GetPlayerData().hasAbility;
        bool hasValidTarget = SelectTarget() != null;

        NotifyAvailability("ProjectileStrategy", hasMissile && hasValidTarget);
    }

    public override void Execute()
    {
        if (targetPlayer == null || !aiController.GetPlayerData().hasAbility)
        {
            NotifyAvailability("ProjectileStrategy", false);
            return;
        }

        if (isAligned)
        {
            LaunchProjectile();
            NotifyAvailability("ProjectileStrategy", false); // Complete the strategy
        }
        else
        {
            MoveToAlign();
        }
    }

    public override void Exit()
    {
        targetPlayer = null;
        isAligned = false;
    }

    private PlayerData SelectTarget()
    {
        PlayerData playerData = aiController.GetPlayerData();
        float closestDistance = float.MaxValue;

        foreach (var opponent in playerData.Opponents)
        {
            float distance = Vector2Int.Distance(playerData.CurrentGridPosition, opponent.CurrentGridPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetPlayer = opponent;
            }
        }

        return targetPlayer;
    }

    private void MoveToAlign()
    {
        // AI's current position and direction
        Vector3 aiPosition = aiController.transform.position;
        Vector3 aiDirection = aiController.transform.forward;

        // Opponent's position
        Vector3 opponentPosition = targetPlayer.transform.position;

        // Calculate direction to the opponent
        Vector3 directionToOpponent = (opponentPosition - aiPosition).normalized;

        // Check if the opponent is directly in front of the AI
        float alignmentThreshold = 0.95f;
        if (Vector3.Dot(aiDirection, directionToOpponent) > alignmentThreshold)
        {
            isAligned = true;
            return; // Stop moving; aligned
        }

        // Not aligned; calculate the next movement position
        Vector2Int aiGridPos = aiController.GetPlayerData().CurrentGridPosition;
        Vector2Int opponentGridPos = targetPlayer.CurrentGridPosition;

        // Determine the best axis to move along
        Vector2Int targetGridPos;
        if (Mathf.Abs(aiGridPos.x - opponentGridPos.x) > Mathf.Abs(aiGridPos.y - opponentGridPos.y))
        {
            // Move along X-axis 
            targetGridPos = new Vector2Int(opponentGridPos.x, aiGridPos.y);
        }
        else
        {
            // Move along Y-axis (Z-axis in world space)
            targetGridPos = new Vector2Int(aiGridPos.x, opponentGridPos.y);
        }

        Node alignmentNode = aiController.GetGridSystem().GetNodeAtPosition(targetGridPos);
        if (alignmentNode != null)
        {
            aiController.SetTargetNode(alignmentNode);
        }
    }

    private void LaunchProjectile()
    {
      //  aiController.GetPlayerData().ActivateCollectable();
        //aiController.GetPlayerData().UseAbility(); // Consume the missile
    }
}
