using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowHeadXR : MonoBehaviour
{
    public float distanceFromHead; // Distance from the head to position the canvas
    public Vector3 offset = Vector3.zero; // Offset from the head's position

    private Transform headTransform;

    void Start()
    {
        // Locate the main camera tagged as "MainCamera"
        headTransform = Camera.main != null ? Camera.main.transform : null;

        // Log error if Main Camera is not found
        if (headTransform == null)
        {
            Debug.LogError("Main Camera not found. Ensure your XR Origin has a camera tagged as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (headTransform != null)
        {
            // Position the canvas at a set distance in front of the head
            Vector3 newPosition = headTransform.position + headTransform.forward * distanceFromHead;
            transform.position = newPosition + offset;

            // Make the canvas face the head
            transform.rotation = Quaternion.LookRotation(transform.position - headTransform.position);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene("scene manager");
        }
        
    }
}
