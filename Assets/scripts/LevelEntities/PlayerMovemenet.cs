using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(PlayerAbilityActivator))]
public class PlayerMovement : MonoBehaviour
{
    private Movement movement; // Movement logic handling
    private Vector2Int inputDirection = Vector2Int.zero; // Direction the player is moving in
    private PlayerData playerData; // Player data including speed, points, etc.
    private Animator animator; // Animator component for handling animations

    private void Awake()
    {
        playerData = GetComponent<PlayerData>(); // Get PlayerData component
        movement = GetComponent<Movement>(); // Get Movement component
        animator = GetComponent<Animator>(); // Get Animator component
    }

    private void Update()
    {
        if (!(playerData.State == PlayerState.Active)) { 
     
            animator.SetBool("IsWalking", false); // Set to Idle animation
            return; // Stop movement and animation updates if player is disabled
        }

        if (!movement.IsMoving())
        {
            HandleInput(); // Accept input when not moving
            animator.SetBool("IsWalking", false); // Set to Idle animation
        }
        else
        {
            animator.SetBool("IsWalking", true); // Set to Walking animation
        }
    }


    private void HandleInput()
    {
        inputDirection = Vector2Int.zero;
        if (playerData.State == PlayerState.Disabled) return;
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
