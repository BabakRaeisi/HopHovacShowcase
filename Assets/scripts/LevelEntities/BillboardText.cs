using UnityEngine;

public class BillboardText : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;  // Cache the main camera for efficiency
    }

    private void Update()
    {
        if (mainCamera != null)
        {
            // Make the text face the camera
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}