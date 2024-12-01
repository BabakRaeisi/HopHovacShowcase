using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfindingStrategy
{
    List<PathNode> GeneratePath(Vector2Int start, Vector2Int target);
    List<PathNode> Detour(Vector2Int start, Vector2Int target);
    
}

public class PathNode
{
    public Vector2Int Coordinates { get; set; }
    public bool IsOccupied { get; set; }
    public PathNode Parent { get; set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost => GCost + HCost;

    public PathNode(Vector2Int coordinates, bool isOccupied)
    {
        Coordinates = coordinates;
        IsOccupied = isOccupied;
        Parent = null;
        GCost = 0;
        HCost = 0;
    }
}

public class AStarPathfindingStrategy : IPathfindingStrategy
{
    private GridSystem gridSystem;

    public AStarPathfindingStrategy(GridSystem grid)
    {
        gridSystem = grid;
    }

    public List<PathNode> GeneratePath(Vector2Int start, Vector2Int target)
    {
        List<PathNode> openList = new List<PathNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        PathNode startNode = new PathNode(start, false);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestCostNode(openList);

            if (currentNode.Coordinates == target)
            {
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.Coordinates);

            Vector2Int[] neighbors = GetCardinalNeighbors(currentNode.Coordinates); // Use only cardinal neighbors

            foreach (Vector2Int neighbor in neighbors)
            {
                if (closedList.Contains(neighbor) || !gridSystem.IsValidPosition(neighbor))
                {
                    continue;
                }

                Node gridNode = gridSystem.GetNodeAtPosition(neighbor);
                if (gridNode == null || gridNode.IsOccupied)
                {
                    continue;
                }

                PathNode neighborNode = new PathNode(neighbor, gridNode.IsOccupied)
                {
                    Parent = currentNode,
                    GCost = currentNode.GCost + 1,
                    HCost = GetHeuristic(neighbor, target)
                };

                // Ensure the move is strictly cardinal (no diagonal movements)
                Vector2Int moveDirection = neighbor - currentNode.Coordinates;
                if (Mathf.Abs(moveDirection.x) > 1 || Mathf.Abs(moveDirection.y) > 1)
                {
                    continue;  // Skip diagonal movement
                }

                if (!openList.Exists(node => node.Coordinates == neighbor))
                {
                    openList.Add(neighborNode);
                }
            }

            if (openList.Count == 0)
            {
               
                return new List<PathNode>();
            }
        }

        return new List<PathNode>();
    }

    // Get only cardinal neighbors (up, down, left, right)
    private Vector2Int[] GetCardinalNeighbors(Vector2Int currentPos)
    {
        return new Vector2Int[]
        {
            new Vector2Int(currentPos.x + 1, currentPos.y),  // Right
            new Vector2Int(currentPos.x - 1, currentPos.y),  // Left
            new Vector2Int(currentPos.x, currentPos.y + 1),  // Up
            new Vector2Int(currentPos.x, currentPos.y - 1)   // Down
        };
    }

    private PathNode GetLowestCostNode(List<PathNode> openList)
    {
        PathNode lowestCostNode = openList[0];
        foreach (var node in openList)
        {
            if (node.FCost < lowestCostNode.FCost)
            {
                lowestCostNode = node;
            }
        }
        return lowestCostNode;
    }

    private List<PathNode> ReconstructPath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.Parent;
        }

        return path;
    }

    private float GetHeuristic(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }

    public List<PathNode> Detour(Vector2Int currentPosition, Vector2Int target)
    {
        return GeneratePath(currentPosition, target);
    }
}





