using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform[] playerPositions;  // Assign player positions in the Inspector
    public Animator[] playerAnimators;  // Assign player animators in the Inspector
    public float transitionSpeed = 5f;  // Speed of camera transitions
    public Vector3 cameraOffset = new Vector3(0, 2, 5);  // Offset for proper positioning
    public Vector3 lookOffset = new Vector3(0, 1.5f, 0);  // Offset for LookAt
    public GameData gameData;  // Reference to the GameData script

    private int currentIndex = 0;  // Current player index
    private Vector3 targetPosition;  // Camera's target position
    private Vector3 initialPosition;  // Camera's initial position
    private Quaternion initialRotation;  // Camera's initial rotation

    private ShoulderHeldCamera shoulderHeldCamera;  // Reference to the sway effect
    private Coroutine moveCoroutine;  // Reference to the coroutine to prevent overlap

    void Start()
    {
        // Record the initial position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (playerPositions.Length > 0)
        {
            targetPosition = playerPositions[currentIndex].position + cameraOffset;
        }

        shoulderHeldCamera = GetComponent<ShoulderHeldCamera>();
        if (shoulderHeldCamera != null)
        {
            shoulderHeldCamera.SetActive(true);  // Enable sway by default
        }
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SelectPrevious();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SelectNext();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Select current player
        {
            TriggerJumpForCurrentPlayer();
        }
    }

    public void SelectPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            StartSwitching();
            if (playerAnimators != null && playerAnimators[currentIndex] != null)
            {
                playerAnimators[currentIndex].SetTrigger("NodTrigger");
            }
        }
        gameData.selectedCharacterIndex = currentIndex;
    }

    public void SelectNext()
    {
        if (currentIndex < playerPositions.Length - 1)
        {
            currentIndex++;
            StartSwitching();
            if (playerAnimators != null && playerAnimators[currentIndex] != null)
            {
                playerAnimators[currentIndex].SetTrigger("NodTrigger");
            }
        }
        gameData.selectedCharacterIndex = currentIndex;
    }

    private void StartSwitching()
    {
        targetPosition = playerPositions[currentIndex].position + cameraOffset;

        // Stop any existing coroutine to prevent overlap
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

       
        // Temporarily disable ShoulderHeldCamera during transitions
        if (shoulderHeldCamera != null)
        {
            shoulderHeldCamera.SetActive(false);
        }

        // Start the camera movement coroutine
        moveCoroutine = StartCoroutine(MoveCameraToTarget(targetPosition, playerPositions[currentIndex].position + lookOffset));
    }

    private IEnumerator MoveCameraToTarget(Vector3 targetPos, Vector3 lookAtTarget)
    {
        float elapsedTime = 0f;  // Time elapsed since the start of the transition
        float transitionDuration = 1f / transitionSpeed;  // Total duration for the transition
        Vector3 startingPosition = transform.position;  // Record the starting position
        Quaternion startingRotation = transform.rotation;  // Record the starting rotation

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / transitionDuration);

            transform.position = Vector3.Lerp(startingPosition, targetPos, t);
            Quaternion targetRotation = Quaternion.LookRotation(lookAtTarget - transform.position);
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.LookRotation(lookAtTarget - transform.position);

        if (shoulderHeldCamera != null)
        {
            shoulderHeldCamera.UpdatePositionAndRotation(transform.localPosition, transform.localRotation);
            shoulderHeldCamera.SetActive(true);  // Re-enable sway
        }

        moveCoroutine = null;  // Clear the coroutine reference
    }

    public void GoToFirstPlayer()
    {
        currentIndex = 0;
        gameData.selectedCharacterIndex = currentIndex;
        StartSwitching();
    }

    public void GoToInitialPosition()
    {
       
        // Stop any existing coroutine to prevent overlap
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // Move the camera back to the initial position and look at the initial target
        Vector3 lookTarget = playerPositions[2].position + lookOffset; // Fallback to looking forward
        moveCoroutine = StartCoroutine(MoveCameraToTarget(initialPosition, lookTarget));
    }

    public void TriggerJumpForCurrentPlayer()
    {
        // Trigger jump animation for the currently selected player
        if (playerAnimators != null && playerAnimators[currentIndex] != null)
        {
            playerAnimators[currentIndex].SetTrigger("JumpTrigger");
        }
    }
}
