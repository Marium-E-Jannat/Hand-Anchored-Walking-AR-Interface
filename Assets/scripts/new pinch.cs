using UnityEngine;

public class AttachCanvasToPinch : MonoBehaviour
{
    [SerializeField] private OVRHand handUsedForPinch; // The hand used for pinch detection
    [SerializeField] private Transform canvasTransform; // The transform of the canvas
    [SerializeField] private bool mockPinch = false; // Mock pinch for testing without hardware
    [SerializeField] private Vector3 canvasOffset = Vector3.zero; // Offset of canvas from pinch point

    private bool isPinching;

    void Update()
    {
        // Check if pinching
        if (IsPinching())
        {
            Vector3 currentHandPosition = handUsedForPinch.transform.position;
            AttachCanvasToHand(currentHandPosition);
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

    private void AttachCanvasToHand(Vector3 handPosition)
    {
        // Attach the canvas to the hand position with an optional offset
        canvasTransform.position = handPosition + canvasOffset;
    }
}
