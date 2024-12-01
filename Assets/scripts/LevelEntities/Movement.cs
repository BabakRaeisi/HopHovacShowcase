using UnityEngine;

public class Movement : MonoBehaviour
{
    private GridSystem gridSystem; // Reference to the GridSystem
    public PlayerData playerData; // Reference to PlayerData

    private Vector3 targetPos; // Target position to move towards
    private bool isMoving = false; // Track whether the character is currently moving
    private bool isRotating = false; // Track whether the character is currently rotating
    float gridSize;
    private Quaternion targetRotation; // Target rotation when changing direction
    public Vector3 directionVector;
    private Vector2Int currentGridPos; // Current position in grid coordinates

    private void Awake()
    {
        gridSystem = FindObjectOfType<GridSystem>(); // Get the GridSystem instance
        gridSize = gridSystem.UnityGridSize;
        playerData.CurrentGridPosition = gridSystem.GetPlayerStartingPosition(this.transform);
    }

    private void Start()
    {
        InitializePosition(this.transform);
    }

    public void InitializePosition(Transform playerTransform)
    {
        // Calculate the grid position based on the player's starting world position
        currentGridPos = GetGridPositionFromWorld(playerTransform.position);
        targetPos = playerTransform.position;

        // Mark the initial position in the grid system as occupied by this player
        gridSystem.PlayerManager.TryMovePlayer(playerData, currentGridPos);

        // Set the player's current position in PlayerData
        playerData.CurrentGridPosition = currentGridPos;
    }

    public void MoveTo(Vector2Int direction)
    {
        // Set smooth rotation to face the new direction regardless of whether the move is valid
        directionVector = new Vector3(direction.x, 0, direction.y);
        targetRotation = Quaternion.LookRotation(directionVector);
        isRotating = true;

        // Attempt to move the player to the target grid position
        Vector2Int targetGridPos = currentGridPos + direction;

        // Ask GridSystem if the move is valid and proceed
        if (gridSystem.PlayerManager.TryMovePlayer(playerData, targetGridPos))
        {
            // Perform the actual movement if the position is valid
            targetPos = CalculateWorldPosition(targetGridPos);
            isMoving = true;
        }
        else
        {
            
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            SmoothMove();
        }
        if (isRotating)
        {
            SmoothRotate();
        }
    }

    private void SmoothMove()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * playerData.Speed);
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMoving = false;
            currentGridPos = GetGridPositionFromWorld(targetPos);  // Update the grid position once movement is finished

            // Update PlayerData with the latest position
            playerData.CurrentGridPosition = currentGridPos;

            // Check if the player has reached a node with a collectable
            Node currentNode = gridSystem.GetNodeAtPosition(currentGridPos);
            if (currentNode != null && currentNode.HasCollectable)
            {
                // Collect the collectable
                Collectable collectable = currentNode.GetCollectable();
                if (collectable != null)
                {
                    collectable.Collect(playerData); // Collect and return to pool
                }
            }
        }
    }

    private void SmoothRotate()
    {
        // Smoothly rotate towards the target direction
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * playerData.SpeedRotation);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.rotation = targetRotation;
            isRotating = false;  // Stop rotating once the target rotation is achieved
        }
    }

    private Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / gridSize);
        int y = Mathf.RoundToInt(worldPosition.z / gridSize);
        return new Vector2Int(x, y);
    }

    private Vector3 CalculateWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * gridSize, transform.position.y, gridPosition.y * gridSize);
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
