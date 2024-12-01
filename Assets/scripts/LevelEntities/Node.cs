using System;
using UnityEngine;

[Serializable]
public class Node
{
    public Vector2Int Coordinates { get; private set; }
    public Tile Tile { get; private set; }

    private Collectable collectable;

    public bool HasCollectable
    {
        get { return collectable != null; }  // Check if the node has a collectable
    }

    public void AssignCollectable(Collectable newCollectable)
    {
        collectable = newCollectable;  // Assign a collectable to this node
         
    }

    public Collectable GetCollectable()
    {
        return collectable;  // Return the collectable assigned to this node
    }

    public void ClearCollectable()
    {
        collectable = null;  // Clear the collectable reference when it is collected
    }

    // Occupied status
    public bool IsOccupied { get; set; }
    private PlayerData owner;  // Private field to store the owner

    public PlayerData Owner
    {
        get => owner;
        set
        {
            // Remove from previous owner's list if there was one
            if (owner != null)
                owner.RemoveOwnedTile(this);

            // Update the new owner
            owner = value;

            // Add to new owner's list if there's a new owner
            if (owner != null)
                owner.AddOwnedTile(this);

            // Set color based on the owner or default if no owner
            NodeColor = owner != null ? owner.PlayerColor : Color.gray;
        }
    }

    // Node color updates the tile when changed
    private Color nodeColor;
    public Color NodeColor
    {
        get => nodeColor;
        set
        {
            nodeColor = value;
            Tile.SetColor(nodeColor);  // Automatically update tile appearance
        }
    }

    // Constructor for Node
    public Node(Vector2Int coordinates, Tile tile)
    {
        Coordinates = coordinates;
        Tile = tile;
        NodeColor = Color.gray;  // Default color is white
        IsOccupied = false;
        owner = null;  // No owner at initialization
    }
    public void ResetOwnership()
    {
        // Remove from current owner's list if any
        if (owner != null)
            owner.RemoveOwnedTile(this);

        owner = null;  // No owner
        NodeColor = Color.gray;  // Default color
        IsOccupied = false;       // Not occupied
    }


}
