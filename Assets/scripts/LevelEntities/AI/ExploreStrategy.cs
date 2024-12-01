using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class ExploreStrategy : StrategyBase
{
    private Node targetNode;
  

    public override void Enter()
    {
        SetNewTarget( );
    }

    public override void EvaluateAvailability()
    {
        NotifyAvailability("ExploreStrategy", true);
    }

    public override void Execute()
    {
        if (aiController.ReachedTarget())
        {
            SetNewTarget();
        }
    }

    public override void Exit()
    {
       
    }

    private void SetNewTarget( )
    {
        GridSystem gridSystem = aiController.GetGridSystem();
       
        targetNode = FindNearbyUnoccupiedNode(gridSystem, playerData);

        if (targetNode != null)
        {
            aiController.SetTargetNode(targetNode);
            playerData.TargetNode = targetNode;
        }

    }
    private Node FindNearbyUnoccupiedNode(GridSystem gridSystem, PlayerData playerData)
    {
        Vector2Int currentPos = playerData.CurrentGridPosition;
        int range = 2;

        List<Node> nearbyNodes = new List<Node>();

        // First, attempt to find unoccupied nodes not in OwnedTiles
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2Int checkPos = currentPos + new Vector2Int(x, y);

                if (checkPos != currentPos && gridSystem.IsValidPosition(checkPos))
                {
                    Node node = gridSystem.GetNodeAtPosition(checkPos);
                    if (node != null && !node.IsOccupied && node.Owner == null && !playerData.OwnedTiles.Contains(node))
                    {
                        nearbyNodes.Add(node);
                    }
                }
            }
        }

        // If no nodes are found, loosen restrictions to include OwnedTiles
        if (nearbyNodes.Count == 0)
        {
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    Vector2Int checkPos = currentPos + new Vector2Int(x, y);

                    if (checkPos != currentPos && gridSystem.IsValidPosition(checkPos))
                    {
                        Node node = gridSystem.GetNodeAtPosition(checkPos);
                        if (node != null && !node.IsOccupied)
                        {
                            nearbyNodes.Add(node);
                        }
                    }
                }
            }
        }

        return nearbyNodes.Count > 0 ? nearbyNodes[Random.Range(0, nearbyNodes.Count)] : null;
    }
}

 

 

 

 





