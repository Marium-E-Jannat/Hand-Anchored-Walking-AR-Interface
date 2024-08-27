using UnityEngine;

public class ClosePinch : MonoBehaviour
{
    [SerializeField] private OVRHand handUsedForPinch; // The hand used for pinch detection
    [SerializeField] private bool mockPinch = false; // Mock pinch for testing without hardware
    [SerializeField] private GameObject indexFingerPad;
    // private bool isPinching;
    [SerializeField] private bool farDistance;
    bool wasPinching = false;
    private Transform headTransform;

    void Start(){
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
                if(farDistance){
                    transform.position += transform.forward.normalized*0.5f;
                }
            }
        }
        wasPinching = IsFourFingerPinching();
    }

    private bool IsFourFingerPinching()
    {
        return (handUsedForPinch != null && handUsedForPinch.GetFingerIsPinching(OVRHand.HandFinger.Index)) || mockPinch;
    }
}

