using UnityEngine;
using Oculus.Interaction.Input; // Ensure you have the correct namespace

public class PalmUIController : MonoBehaviour
{
    public GameObject palmUICanvas; // Reference to the UI canvas
    public OVRSkeleton handSkeleton; // Reference to the hand skeleton
    public float showThresholdAngle = 30f; // Angle threshold to show the UI
    public float distanceFromPalm = 0.1f; // Distance from palm to place UI

    void Update()
    {
        if (handSkeleton.IsDataValid && handSkeleton.IsDataHighConfidence)
        {
            UpdatePalmUI();
        }
        else
        {
            palmUICanvas.SetActive(false);
        }
    }

    void UpdatePalmUI()
    {
        // Get the palm normal
        Vector3 palmNormal = GetPalmNormal(handSkeleton);
        // Get the headset's forward direction
        Vector3 headsetForward = Camera.main.transform.forward;

        // Calculate the angle between palm normal and headset forward
        float angle = Vector3.Angle(palmNormal, headsetForward);

        // Show or hide the UI based on the angle
        if (angle < showThresholdAngle)
        {
            palmUICanvas.SetActive(true);

            // Position the canvas in front of the palm
            palmUICanvas.transform.position = handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform.position + palmNormal * distanceFromPalm;
            // Rotate the canvas to face the headset
            palmUICanvas.transform.rotation = Quaternion.LookRotation(palmNormal);
        }
        else
        {
            palmUICanvas.SetActive(false);
        }
    }

    Vector3 GetPalmNormal(OVRSkeleton skeleton)
    {
        // Assuming the palm normal can be approximated by the cross product of the thumb and middle finger directions
        Vector3 thumbPosition = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb3].Transform.position;
        Vector3 middlePosition = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle3].Transform.position;
        Vector3 wristPosition = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform.position;

        // Vector from wrist to thumb and wrist to middle
        Vector3 wristToThumb = thumbPosition - wristPosition;
        Vector3 wristToMiddle = middlePosition - wristPosition;

        // Palm normal is the cross product of these vectors
        return Vector3.Cross(wristToThumb, wristToMiddle).normalized;
    }
}
