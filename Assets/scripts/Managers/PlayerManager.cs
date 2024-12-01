using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private Dictionary<PlayerData, Node> playerCurrentNodes = new Dictionary<PlayerData, Node>();
    private GridSystem gridSystem;

    public PlayerManager(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }

    // 
    public bool TryMovePlayer(PlayerData player, Vector2Int newCoords)
    {
        if (gridSystem.IsValidPosition(newCoords))
        {
            if (playerCurrentNodes.TryGetValue(player, out Node currentNode))
            {
                currentNode.IsOccupied = false;
            }

            Node targetNode = gridSystem.GetNodeAtPosition(newCoords);

            // Update target node ownership
            targetNode.Owner = player;

            targetNode.IsOccupied = true;
            playerCurrentNodes[player] = targetNode;

            player.CurrentGridPosition = newCoords;

            return true;
        }

        return false;
    }

    // Method to set opponents for each player and identify their main competitor
    public void UpdateOpponents(List<PlayerData> allPlayers)
    {
        foreach (PlayerData player in allPlayers)
        {
       
            var opponents = new List<PlayerData>(allPlayers);
            player.OwnedTiles.Clear();
            player.ResetPoints();
            opponents.Remove(player);
            player.SetOpponents(opponents);

        }
    }

    
   
}
