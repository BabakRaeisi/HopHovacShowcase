using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMovement : MonoBehaviour
{
    private Movement movement; // Movement logic handling
    private Vector2Int inputDirection = Vector2Int.zero; // The direction the player is moving in
    public PlayerData playerData; // Player data including speed, points, etc.
    
    private void Awake()
    {
        movement = GetComponent<Movement>(); // Get the Movement component
    }

    private void Start()
    {
      
    }

    private void Update()
    {
        if (!movement.IsMoving())
        {
            HandleInput(); // Only accept new input when not moving
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            playerData.hasAbility = false;
        }
    }

    private void HandleInput()
    {
        inputDirection = Vector2Int.zero;

        // Use keyboard or controller inputs to get the direction
        if (Input.GetKey(KeyCode.W))
        {
            inputDirection = Vector2Int.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            inputDirection = Vector2Int.down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            inputDirection = Vector2Int.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            inputDirection = Vector2Int.right;
        }

        if (inputDirection != Vector2Int.zero)
        {
            movement.MoveTo(inputDirection); // Call MoveTo in Movement script to handle movement
        }
    }
}