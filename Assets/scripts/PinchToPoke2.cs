using UnityEngine;

/*public class PinchMoveCanvas : MonoBehaviour
{
    [SerializeField] private OVRHand handUsedForPinch; // The hand used for pinch detection
    [SerializeField] private Transform canvasTransform; // The transform of the canvas
    [SerializeField] private float moveSpeed = 1.0f; // Speed of the canvas movement
    [SerializeField] private bool mockPinch = false; // Mock pinch for testing without hardware

    private bool isPinching;
    private Vector3 lastHandPosition;
    private Vector3 canvasStartPosition;

    void Start()
    {
        // Initialize the starting position of the canvas
        canvasStartPosition = canvasTransform.position;
    }

    void Update()
    {
        // Check if pinching
        if (IsPinching())
        {
            Vector3 currentHandPosition = handUsedForPinch.transform.position;

            if (isPinching)
            {
                Vector3 handDelta = currentHandPosition - lastHandPosition;
                MoveCanvasFromBottomLeft(handDelta);
            }

            lastHandPosition = currentHandPosition;
            isPinching = true;
        }
        else
        {
            isPinching = false;
        }
    }

    private bool IsPinching()
    {
        return (handUsedForPinch != null && handUsedForPinch.GetFingerIsPinching(OVRHand.HandFinger.Index)) || mockPinch;
    }

    private void MoveCanvasFromBottomLeft(Vector3 handDelta)
    {
        // Calculate the new position of the canvas based on the hand's delta movement
        Vector3 newCanvasPosition = canvasTransform.position + handDelta * moveSpeed;

        // Move the canvas
        canvasTransform.position = newCanvasPosition;
    }
}*/


public class PinchMoveCanvas2 : MonoBehaviour
{
    [SerializeField] private OVRHand handUsedForPinch; // The hand used for pinch detection
    [SerializeField] private Transform canvasTransform; // The transform of the canvas
    [SerializeField] private float moveSpeed = 1.0f; // Speed of the canvas movement
    [SerializeField] private float rotationSpeed = 50.0f; // Speed of the canvas rotation
    [SerializeField] private bool mockPinch = false; // Mock pinch for testing without hardware

    private bool isPinching;
    private Vector3 lastHandPosition;

    void Update()
    {
        // Check if pinching
        if (IsPinching())
        {
            Vector3 currentHandPosition = handUsedForPinch.transform.position;

            if (isPinching)
            {
                Vector3 handDelta = currentHandPosition - lastHandPosition;
                MoveCanvasFromBottomLeft(handDelta);
                RotateCanvas(handDelta);
            }

            lastHandPosition = currentHandPosition;
            isPinching = true;
        }
        else
        {
            isPinching = false;
        }
    }

    private bool IsPinching()
    {
        return (handUsedForPinch != null && handUsedForPinch.GetFingerIsPinching(OVRHand.HandFinger.Index)) || mockPinch;
    }

    private void MoveCanvasFromBottomLeft(Vector3 handDelta)
    {
        // Calculate the new position of the canvas based on the hand's delta movement
        Vector3 newCanvasPosition = canvasTransform.position + handDelta * moveSpeed;

        // Move the canvas
        canvasTransform.position = newCanvasPosition;
    }

    private void RotateCanvas(Vector3 handDelta)
    {
        // Use the horizontal movement of the hand to determine rotation angle
        float rotationAngle = handDelta.x * rotationSpeed * Time.deltaTime;

        // Rotate the canvas around the Z axis (to make it more visible, change as needed)
        canvasTransform.Rotate(Vector3.forward, rotationAngle);
    }
}
