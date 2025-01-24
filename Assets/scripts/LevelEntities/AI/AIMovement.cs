using System;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    private GridSystem gridSystem;
    private PlayerData playerData;
    private Movement movement;
    private Animator animator;
    public bool HasTarget { get { return hasTarget; } set { hasTarget = value; } }
    public Node TargetNode { get { return targetNode; } set { targetNode = value; } }

    private bool hasTarget = false;
    private Node targetNode;
    private int currentStepIndex = 0;
    [SerializeField] private List<PathNode> path = new List<PathNode>();

    private Vector2Int previousGridPos;
    private float stuckTimer = 0f;
    private float maxStuckTime = .75f;

    private IPathfindingStrategy pathfindingStrategy;

    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
        animator = GetComponent<Animator>();
        gridSystem = FindAnyObjectByType<GridSystem>();
        movement = GetComponent<Movement>();
        pathfindingStrategy = new AStarPathfindingStrategy(gridSystem);
      
    }
    

    private void Update()
    {
        if (playerData.CurrentGridPosition == previousGridPos)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > maxStuckTime)
            {
              
                stuckTimer = 0f;
                SetTargetNode(targetNode);  // Recalculate the path to the current target
            }
        }
        else
        {
            stuckTimer = 0f;
            previousGridPos = playerData.CurrentGridPosition;
        }

        MoveAlongPath();
    }

    public bool ReachedTarget()
    {
        if (targetNode == null) return false;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = targetNode.Tile.transform.position;
        float threshold = 0.5f;

        bool reached = Vector3.Distance(currentPosition, targetPosition) <= threshold;
        if (reached)
        {
          
            hasTarget = false;  // Reset `hasTarget` when target is reached
        }

        return reached;
    }


    public void SetTargetNode(Node newTargetNode)
    {
        if (newTargetNode == null)
        {
          
            return;
        }

        targetNode = newTargetNode;
        hasTarget = true;

        // Set PlayerData.TargetNode to the new target
        playerData.TargetNode = targetNode;

        // Generate the path excluding the current position node
        path = pathfindingStrategy.GeneratePath(playerData.CurrentGridPosition, targetNode.Coordinates);

        if (path.Count > 0 && path[0].Coordinates == playerData.CurrentGridPosition)
        {
            path.RemoveAt(0);  // Remove the first step if it is the current position
        }

        currentStepIndex = 0;
        
    }

    private void MoveAlongPath()
    {
        if (!(playerData.State==PlayerState.Active)){ 
            animator.SetBool("IsWalking", false);
            return; }
        if (currentStepIndex < path.Count && !movement.IsMoving())
        {
            Vector2Int nextStep = path[currentStepIndex].Coordinates;
            Node nextNode = gridSystem.GetNodeAtPosition(nextStep);

            if (nextNode != null && !nextNode.IsOccupied)
            {
                movement.MoveTo(nextStep - playerData.CurrentGridPosition);
                playerData.CurrentGridPosition = nextStep;
                currentStepIndex++;

                if (currentStepIndex >= path.Count)
                {
                    hasTarget = false;
                }
            }
            else
            {
               
                path = pathfindingStrategy.Detour(playerData.CurrentGridPosition, targetNode.Coordinates);
                currentStepIndex = 0;  // Reset path index after recalculating
            }
        }
        animator.SetBool("IsWalking", true);
    }
}
