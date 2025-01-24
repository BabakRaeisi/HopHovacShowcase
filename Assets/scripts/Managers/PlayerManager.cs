using System.Collections.Generic;
using System.Linq;
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
    public void ManagePlayers(List<PlayerData> allPlayers)
    {
        foreach (PlayerData player in allPlayers)
        {
            // Reset player state
            player.OwnedTiles.Clear();
            player.ResetPoints();

            // Assign initial grid position
            player.InitialGridPosition = player.CurrentGridPosition;
            TryMovePlayer(player, player.CurrentGridPosition);

            // Link opponents with UI indices
            var opponentsWithUI = allPlayers
                .Where(opponent => opponent != player) // Exclude the player itself
                .Select(opponent => (opponent, opponent.ImageIndex)) // Map opponents with ImageIndex
                .ToList();
            player.SetOpponents(opponentsWithUI);

            // Disable player until game starts
            player.State = PlayerState.Locked;
        }
    }



}
