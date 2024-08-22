using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ButtonInteractionHandler : MonoBehaviour
{
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.green;
    public Color genericColor;
    //private Button lastButton;
    public static bool clicked = false;
    public static int PinchCounter;
    public static float distance;

    [SerializeField] private OVRHand rightHand;  // Reference to the right hand
    [SerializeField] private bool mockHandUsedForPinchSelection; // Mock pinch selection for testing
    [SerializeField] private bool allowPinchSelection = true; // Allow pinch selection
    [SerializeField] private Canvas canvas; // Reference to the Canvas

    [SerializeField] private Button startButton;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    public EventSystem eventSystem;

    private void Start()
    {
        // Get the GraphicRaycaster and EventSystem from the Canvas
        raycaster = canvas.GetComponent<GraphicRaycaster>();

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

        ActivateStart();
    }

    public void HandleButtonClick(Button button)
    {
        if (button != null)
        {
            HandleButtonInteraction(button);
        }
    }

    private IEnumerator HandleStartButton(Button button){
        yield return new WaitUntil(()=>IsRightHandPinching() == false);
        fittsrecorder2.quizInProgress  = true;
        // start timer
        fittsrecorder2.startTime = DateTime.Now;
        // Deactivate start button while quiz is in process
        button.GetComponent<Button>().interactable = false;
        button.GetComponent<Image>().color = defaultColor;
    }

    public void ActivateStart(){
        if(startButton != null){
            startButton.interactable = true;
            startButton.GetComponent<Image>().color = Color.red;
        }else{
            Debug.Log("Start button in hand tracking handler is not assigned.");
        }
    }

    private void HandleButtonInteraction(Button button)
    {
        // Ensure only one button is selected at a time
        //if (lastButton != null && lastButton != button)
        //{
        //    ResetButtonColor(lastButton);
        //}

        // FIXME: Move this fittrecorder2 if possible
        //ResetButtonColor(button);

        // Handle button click
        distance = 0;
        Vector3 buttonCenter = button.GetComponent<RectTransform>().position;
        distance = Vector3.Distance(Camera.main.transform.position, buttonCenter);

        // QUESTION: Should the button be colored green for correctness
        // Update button color to something that means CORRECT
        // Image image = button.GetComponent<Image>();
        // genericColor = image.color;
        // SetButtonColor(button, selectedColor);

        string buttonName = button.name;
        if (buttonName.StartsWith("option") && int.TryParse(buttonName.Substring(6), out int buttonNumber))
        {
            Recorder.selec = buttonNumber;
            fittsrecorder.selec=buttonNumber;
            Recorder.btnsave = buttonNumber;
            if (buttonNumber == 0){
                Debug.Log("HAND_TRACKING: I clicked Start button");
                StartCoroutine(HandleStartButton(button));
            }
            Debug.Log($"HAND_TRACKING: Button selected is {Recorder.selec}");
        }

        //lastButton = button;
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
        // if (IsRightHandPinching())
        // {
        //     HandlePinch();
        // }
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

        Debug.Log("HAND_TRACKING: " + results.Count);

        // Iterate through the results
        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            Debug.Log("HAND_TRACKING: " + button.gameObject.name);
            if (button != null)
            {
                HandleButtonClick(button);
                PinchCounter++;
                break; // Exit loop after first button click
            }
        }
    }
}
