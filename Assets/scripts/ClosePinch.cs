using UnityEngine;

public class ClosePinch : MonoBehaviour
{
    [SerializeField] private OVRHand handUsedForPinch; // The hand used for pinch detection
    [SerializeField] private Transform canvasTransform; // The transform of the canvas
    [SerializeField] private float moveSpeed = 1.0f; // Speed of the canvas movement
    [SerializeField] private float rotationSpeed = 50.0f; // Speed of the canvas rotation
    [SerializeField] private bool mockPinch = false; // Mock pinch for testing without hardware
    [SerializeField] private GameObject indexFingerPad;
    [SerializeField] private GameObject centerHead;
    // private bool isPinching;
    [SerializeField] private bool closeDistance;
    bool wasPinching = false;
    private Transform headTransform;
    Vector3 canvasHalfSize;

    void Start(){
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta * rectTransform.localScale;
        canvasHalfSize = new Vector3(size.x / 2.0f, size.y / 2.0f, 0);
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
        // Check if pinching
        if (headTransform != null && IsFourFingerPinching())
        {
            if(!wasPinching){
                transform.position = indexFingerPad.transform.position;
                transform.rotation = Quaternion.LookRotation(transform.position - headTransform.position);
            }
        }
        wasPinching = IsFourFingerPinching();
    }

    private bool IsFourFingerPinching()
    {
        return (handUsedForPinch != null && handUsedForPinch.GetFingerIsPinching(OVRHand.HandFinger.Index)) || mockPinch;
    }
}

