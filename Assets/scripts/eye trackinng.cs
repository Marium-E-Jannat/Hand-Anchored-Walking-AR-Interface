using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class EyeTracking : MonoBehaviour
{
    [SerializeField] private Color rayColorDefaultState = Color.yellow;
    [SerializeField] public Color genericColor;
    [SerializeField] private Color rayColorPinchState = Color.green;
    [SerializeField] private LayerMask layersToInclude;
    [SerializeField] private OVRHand handUsedForPinchSelection;
    [SerializeField] private bool mockHandUsedForPinchSelection;
    [SerializeField] private bool allowPinchSelection; // Define allowPinchSelection here
    //
    private bool _isPinching = false;
    public static bool clicked=false;
    private LineRenderer lineRenderer;
    private Button lastButton;
    private bool isPinching;
    public static int PinchCounter;
    public static float distance;
    


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();


    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(0, 0, 1.0f));
    }

    private void FixedUpdate()
    {
        if (IsPinching())
        {
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(transform.position, transform.forward, out hit, 10.0f, layersToInclude);
            if(!_isPinching){
                PinchCounter++;
                _isPinching=true;
                if (hitSomething)
                {
                    HandlePinch(hit);
                }
            } 
        }
        else{
            _isPinching=false;
        }
       /* else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;

            // Reset lastButton only when not pinching
            if (!isPinching && lastButton != null)
            {
                SetButtonColors(lastButton, rayColorDefaultState);
                lastButton = null;
            }
        }*/
    }

    private void HandlePinch(RaycastHit hit)
    {
       /* lineRenderer.startColor = rayColorPinchState;
        lineRenderer.endColor = rayColorPinchState;
        */
        
        Button button = hit.transform.GetComponent<Button>();
        if (button != null)
        {
            // Change button color only once when pinching starts
            distance=0;
            Vector3 buttonCenter = button.GetComponent<RectTransform>().position;
             distance = Vector3.Distance(hit.point, buttonCenter);
            
            if (button != lastButton)
            {
                 if(lastButton!=null)
                 {
                    Image imaged=lastButton.GetComponent<Image>();
                    imaged.color=Color.white;
                     TextMeshProUGUI texts = lastButton.GetComponentInChildren<TextMeshProUGUI>();
                        texts.color=Color.black;
                         string buttonName=lastButton.name;
                    if (int.TryParse(buttonName.Substring(6), out int buttonNumber))
                    {
                        Recorder.btnsave= buttonNumber;
                        Debug.Log(Recorder.selec);
               
                     }
                 }
                 Image image = button.GetComponent<Image>();
                genericColor=image.color;
                clicked=true;
                SetButtonColors(button, rayColorPinchState);
               
                
                lastButton = button;
            }
            isPinching = true;
        }
        else
        {
            isPinching = false;
        }
    }

    private bool IsPinching()
    {
        return (allowPinchSelection && handUsedForPinchSelection != null &&
                handUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index)) ||
               mockHandUsedForPinchSelection;
    }

    private void SetButtonColors(Button button, Color color)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
        string buttonName = button.name;
        if (buttonName.StartsWith("option"))
        {
            if (int.TryParse(buttonName.Substring(6), out int buttonNumber))
            {
                Recorder.selec = buttonNumber;
                Debug.Log(Recorder.selec);
               
            }
        }
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = color;
        }
    }
}
