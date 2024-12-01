using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
    public class GridUtility
    {
        private Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

        public Dictionary<Vector2Int, Node> InitializeGrid(List<GameObject> tileGameObjects, int unityGridSize)
        {
            foreach (GameObject tileObject in tileGameObjects)
            {
                AddNodeToGrid(tileObject, unityGridSize);
            }
            return grid;
        }

        // Adds a tile to the grid and sets its coordinates
        private void AddNodeToGrid(GameObject tileObject, int unityGridSize)
        {
            Tile tile = tileObject.GetComponent<Tile>();
            if (tile != null)
            {
                Vector2Int coordinates = GetTileCoordinates(tileObject.transform.position, unityGridSize);
                Node node = CreateNode(coordinates, tile);
                node.IsOccupied = false;  // Initialize node as unoccupied
                grid.Add(coordinates, node);
            }
            else
            {
                Debug.LogError("Tile component missing on tile object: " + tileObject.name);
            }
        }

        // Converts world position to grid coordinates
        private Vector2Int GetTileCoordinates(Vector3 worldPosition, int unityGridSize)
        {
            int x = Mathf.RoundToInt(worldPosition.x / unityGridSize);
            int y = Mathf.RoundToInt(worldPosition.z / unityGridSize);
            return new Vector2Int(x, y);
        }

        // Creates a new Node for the grid
        private Node CreateNode(Vector2Int coordinates, Tile tile)
        {
            return new Node(coordinates, tile);
        }
    }
 