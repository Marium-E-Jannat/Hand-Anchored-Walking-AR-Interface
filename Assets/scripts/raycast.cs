using UnityEngine;

public class PalmRaycastDetector : MonoBehaviour
{
    public GameObject palmUICanvas; // Reference to the UI canvas
    public Transform palmTransform; // Transform of the palm
    public Transform cameraTransform; // Transform of the main camera
    public float rayLength = 0.5f; // Length of the ray
    public LayerMask detectionLayer; // Layer mask to detect the collider

    void Update()
    {
        CastRayFromPalm();
    }

    void CastRayFromPalm()
    {
        // Define ray starting at the palm and directed outward
        Ray ray = new Ray(palmTransform.position, palmTransform.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, rayLength, detectionLayer))
        {
            // If the ray hits the collider, show the canvas
            palmUICanvas.SetActive(true);

            // Position the canvas slightly in front of the palm
            palmUICanvas.transform.position = palmTransform.position + palmTransform.forward * 0.1f;

            // Rotate the canvas to face the camera
            palmUICanvas.transform.LookAt(cameraTransform);
        }
        else
        {
            // If the ray does not hit, hide the canvas
            palmUICanvas.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        // Draw the ray for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawRay(palmTransform.position, palmTransform.forward * rayLength);
    }
}
