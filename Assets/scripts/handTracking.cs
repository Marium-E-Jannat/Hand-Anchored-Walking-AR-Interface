using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonInteractionHandler : MonoBehaviour
{
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.green;
    public Color genericColor;
    private Button lastButton;
    public static bool clicked = false;
    public static int PinchCounter;
    public static float distance;
    private bool _isPinching = false;

    [SerializeField] private OVRHand rightHand;  // Reference to the right hand
    [SerializeField] private bool mockHandUsedForPinchSelection; // Mock pinch selection for testing
    [SerializeField] private bool allowPinchSelection = true; // Allow pinch selection
    [SerializeField] private Canvas canvas; // Reference to the Canvas

    [SerializeField] private Button startButton;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Start()
    {
        // Get the GraphicRaycaster and EventSystem from the Canvas
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();

        if (raycaster == null)
        {
            Debug.LogError("GraphicRaycaster component is missing from the Canvas.");
            return;
        }

        if (eventSystem == null)
        {
            Debug.LogError("EventSystem component is missing from the GameObject.");
            return;
        }

        // Subscribe to button click events
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.onClick.AddListener(() => HandleButtonInteraction(button));
        }
    }

    public void OnButtonClick(GameObject buttonObject)
    {
        Debug.Log("hey i am working");
        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            HandleButtonInteraction(button);
        }
    }

    private IEnumerator HandleStartButton(Button button){
        yield return new WaitUntil(()=>IsRightHandPinching() == false);
        fittsrecorder2.quizInProgress  = true;
        fittsrecorder2.startTime = Time.time;
        // Deactivate start button while quiz is in process
        button.GetComponent<Button>().interactable = false;
    }

    public void ActivateStart(){
        if(startButton != null){
            startButton.interactable = true;
        }else{
            Debug.Log("Start button in hand tracking handler is not assigned.");
        }
    }

    private void HandleButtonInteraction(Button button)
    {
        // Ensure only one button is selected at a time
        if (lastButton != null && lastButton != button)
        {
            ResetButtonColor(lastButton);
        }

        // Handle button click
        distance = 0;
        Vector3 buttonCenter = button.GetComponent<RectTransform>().position;
        distance = Vector3.Distance(Camera.main.transform.position, buttonCenter);

        // Update button colors
        Image image = button.GetComponent<Image>();
        genericColor = image.color;
        SetButtonColor(button, selectedColor);

        string buttonName = button.name;
        if (buttonName.StartsWith("option") && int.TryParse(buttonName.Substring(6), out int buttonNumber))
        {
            Recorder.selec = buttonNumber;
            fittsrecorder.selec=buttonNumber;
            Recorder.btnsave = buttonNumber;
            if (buttonNumber == 0){
                StartCoroutine(HandleStartButton(button));
            }
            Debug.Log($"Button selected: {Recorder.selec}");
        }

        lastButton = button;
        clicked = true;
        PinchCounter++;
    }

    private void ResetButtonColor(Button button)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = defaultColor;
        }
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = Color.black;
        }
    }

    private void SetButtonColor(Button button, Color color)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = color;
        }
    }

    private bool IsRightHandPinching()
    {
        return (allowPinchSelection && rightHand != null &&
                rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index)) ||
               mockHandUsedForPinchSelection;
    }

    private void Update()
    {
        if (lastButton != null){
            ResetButtonColor(lastButton);
        }
        if (IsRightHandPinching())
        {
            HandlePinch();
        }
        else
        {
            _isPinching = false;
        }
    }

    private void HandlePinch()
    {
        // Create a new PointerEventData
        pointerEventData = new PointerEventData(eventSystem)
        {
            // Set the position of the pointer
            position = Camera.main.WorldToScreenPoint(rightHand.transform.position)
        };

        // Create a list to hold the results of the raycast
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the GraphicRaycaster and the pointer event data
        raycaster.Raycast(pointerEventData, results);

        // Iterate through the results
        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                OnButtonClick(button.gameObject);
                _isPinching = true;
                PinchCounter++;
                break; // Exit loop after first button click
            }
        }
    }
}
