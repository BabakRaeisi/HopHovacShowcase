using System;
using UnityEngine;

[Serializable]
public class Node
{
    public Vector2Int Coordinates { get; private set; }
    public Tile Tile { get; private set; }

    private Collectable collectable;

    public bool HasCollectable => collectable != null;

    public void AssignCollectable(Collectable newCollectable)
    {
        collectable = newCollectable;
    }

    public Collectable GetCollectable()
    {
        return collectable;
    }

    public void ClearCollectable()
    {
        collectable = null;
    }

    // Occupied status
    public bool IsOccupied { get; set; }
    private PlayerData owner;

    public PlayerData Owner
    {
        get => owner;
        set
        {
            if (owner != null)
                owner.RemoveOwnedTile(this);

            owner = value;

            if (owner != null)
                owner.AddOwnedTile(this);

            if (owner != null)
            { Tile.SetMaterial(owner.PlayerMaterial); } else { Tile.ResetMaterial();} 
        }
    }

    // Node material updates the tile when changed
    private Material nodeMaterial;
    public Material NodeMaterial
    {
        get => nodeMaterial;
        set
        {
            nodeMaterial = value;
            Tile.SetMaterial(nodeMaterial); // Automatically update tile appearance
        }
    }

    // Default material for unowned nodes
    public static Material DefaultMaterial { get; set; }

    // Constructor for Node
    public Node(Vector2Int coordinates, Tile tile)
    {
        Coordinates = coordinates;
        Tile = tile;
        DefaultMaterial = Tile.UnoccupiedMat;
        NodeMaterial = DefaultMaterial; // Default material for unowned nodes
        IsOccupied = false;
        owner = null;
    }

    public void ResetOwnership()
    {
        if (owner != null)
            owner.RemoveOwnedTile(this);

        owner = null;
        NodeMaterial = DefaultMaterial;
        IsOccupied = false;
    }
}
