using UnityEngine;

public class ShoulderHeldCamera : MonoBehaviour
{
    public Transform cameraTransform;  // Reference to the camera
    public float swayAmount = 0.1f;    // Sway amount
    public float swaySpeed = 2.0f;     // Sway speed
    public float rotationAmount = 0.5f; // Rotation sway amount
    public float rotationSpeed = 2.0f; // Rotation sway speed

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private bool isActive = true;  // Controls whether SHC should operate

    void Start()
    {
        // Cache the initial local position and rotation relative to the camera parent
        initialLocalPosition = cameraTransform.localPosition;
        initialLocalRotation = cameraTransform.localRotation;
    }

    void Update()
    {
        if (!isActive) return;  // Skip updates if inactive

        // Apply sway to position
        Vector3 swayPosition = new Vector3(
            Mathf.Sin(Time.time * swaySpeed) * swayAmount,
            Mathf.Sin(Time.time * swaySpeed * 0.5f) * swayAmount,
            0);

        cameraTransform.localPosition = initialLocalPosition + swayPosition;

        // Apply sway to rotation
        Quaternion swayRotation = Quaternion.Euler(
            Mathf.Sin(Time.time * rotationSpeed) * rotationAmount,
            Mathf.Sin(Time.time * rotationSpeed * 0.7f) * rotationAmount,
            0);

        cameraTransform.localRotation = initialLocalRotation * swayRotation;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (!isActive)
        {
            // Stop sway, keep the last active position and rotation
            initialLocalPosition = cameraTransform.localPosition;
            initialLocalRotation = cameraTransform.localRotation;
        }
    }

    public void UpdatePositionAndRotation(Vector3 newLocalPosition, Quaternion newLocalRotation)
    {
        // Adjust the initial position and rotation to match the new camera position
        initialLocalPosition = newLocalPosition;
        initialLocalRotation = newLocalRotation;
    }
}
