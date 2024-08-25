using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Events;
using System;
using TMPro;

public class QuizRecorder : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] optionButtons;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    public Color colorAtStill;
    public static int selec;
    public static DateTime startTime;
    [SerializeField] private OVRHand rightHand;
    public bool quizInProgress;
    int quizNumber = 0;
    public UnityEvent startNewQuiz = new UnityEvent();
    [SerializeField] private GameObject startPanel; 
    [SerializeField] private GameObject questionPanel; 
    private Button prevButtonClicked;
    private bool allowSubmit = false;
    private int pinchCount = 0;
    private DateTime optionClickedTime;
    private bool wasPinching;
    [Header("Center anchors")]
    [SerializeField] private Transform head;
    const string instruction1 = "Press Start to view the question.";
    const string instruction2 = "Reorient the canvas to the center and look forward to continue.";

    enum status {
        ERROR,
        SUCCESS
    }
    public float distanceUpperThreshold = 0.2f, distanceLowerThreshold = 0.05f;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();
    private int correctAnswer;
    private int pickedAnswer;
    [SerializeField] private Button submitButton;
    TMP_Text indicator;

    void Start()
    {
        firebaseManager = FirebaseSceneManager.Instance;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
        if(head == null){
            Debug.LogError("Head anchor is not set properly.");
        }
        if(startPanel.transform.Find("Instruction") != null){
            indicator= startPanel.transform.Find("Instruction").GetComponent<TMP_Text>();
        }else{
            Debug.LogError("Start panel doesn't have Instruction child.");
        }
        colorAtStill = optionButtons[1].colors.normalColor;
        quizInProgress = false;
        questionPanel.SetActive(false);
        startPanel.SetActive(false);
    }

    private void Update(){
        if (quizInProgress){
            startPanel.SetActive(false);
            questionPanel.SetActive(true);
            ShowQuestion();
            if(AnswerLocation.Instance != null){
                AnswerLocation.Instance.SetText(Questions.Instance.options[quizNumber, correctAnswer]);
            }else{
                Debug.LogError("Answer location is not assigned to an object");
            }
            if(!allowSubmit){
                submitButton.interactable = false;
            }else{
                submitButton.interactable = true;
            }
            if(IsIdxFingerPinching()){
                pinchCount += 1;
            }
        } else{
            startPanel.SetActive(true);
            questionPanel.SetActive(false);
            if(isHeadCenter()){
                indicator.text = instruction1;
                indicator.color = Color.black;
            }else{
                indicator.text = instruction2;
                indicator.color = Color.yellow;
            }
            if(AnswerLocation.Instance != null){
                AnswerLocation.Instance.SuspendText();
            }else{
                Debug.LogError("Answer location is not assigned to an object");
            }
        }
    }

    private bool isHeadCenter(){
        Vector3 headRotation = head.rotation.eulerAngles;
        return headRotation.y > -10f && headRotation.y < 10f;
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

    private void ResetButtonColor(Button button)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = colorAtStill;
        }
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = Color.black;
        }
    }

    public void OnStartClicked(Button button){
        if(!isHeadCenter()){
            return;
        }
        if(prevButtonClicked != null){
            ResetButtonColor(prevButtonClicked);
        }
        if(quizNumber < Questions.Instance.questions.Length){
            quizInProgress = true;
        }
        prevButtonClicked = button;
        ResetMetrics();
        if(AnswerLocation.Instance != null){
            AnswerLocation.Instance.ResetTxtSide();
        }else{
            Debug.LogError("Answer location is not assigned to an object");
        }
    }

    private void ResetMetrics(){
        pinchCount = 0;
        startTime = DateTime.Now;
        allowSubmit = false;
    }

    public void OnOptionClicked(Button button){
        if(prevButtonClicked != null){
            ResetButtonColor(prevButtonClicked);
        }
        string buttonName = button.name;
        if (buttonName.StartsWith("option") && int.TryParse(buttonName.Substring(6), out int buttonNumber))
        {
            pickedAnswer = buttonNumber;
        }else{
            Debug.LogError("Wrong option button name");
        }
        SetButtonColor(button, Color.green);
        prevButtonClicked = button;
        allowSubmit = true;
        optionClickedTime = DateTime.Now;
    }

    public void OnSubmitClicked(Button button){
        if(!allowSubmit){
            return;
        }
        if(prevButtonClicked != null){
            ResetButtonColor(prevButtonClicked);
        }
        double optionDuration = optionClickedTime.Subtract(startTime).TotalMilliseconds;
        double submitDuration = DateTime.Now.Subtract(optionClickedTime).TotalMilliseconds;
        string statusString;
        if (pickedAnswer != correctAnswer){
            statusString = "Error";
        }else{
            statusString = "Success";
        }

        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "Quiz number", quizNumber},
            { "Status", statusString},
            { "Pinch count", pinchCount},
            { "Duration from start to option", optionDuration},
            { "Duration from option to submit", submitDuration}
        };

        responses.Add(response);
        if(quizNumber == Questions.Instance.questions.Length - 1){
            SaveResponsesToFirebase();
        }
        quizNumber += 1;
        quizInProgress = false;
        prevButtonClicked = button;
    }

    public void ShowQuestion()
    {
        Debug.Log("Showing question at index: " + quizNumber);
        if (Questions.Instance == null || Questions.Instance.questions == null || Questions.Instance.questions.Length == 0)
        {
            Debug.LogError("Questions.Instance or its questions are null or empty.");
            return;
        }
        string question = Questions.Instance.questions[quizNumber];

        if (questionText == null)
        {
            Debug.LogError("questionText is not assigned.");
            return;
        }

        // questionText.text = question;
        questionText.text = "x+0=?";
        // Reset colors for all buttons
        for (int i = 0; i < 4; i++)
        {
            if (optionButtons[i] == null)
            {
                Debug.LogError($"optionButtons[{i}] is not assigned.");
                continue;
            }

            TMP_Text buttonText = optionButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText == null)
            {
                Debug.LogError($"Button at index {i} does not have a TMP_Text component.");
                continue;
            }

            buttonText.text = Questions.Instance.options[quizNumber, i];
            optionButtons[i].interactable = true;
        }
        correctAnswer = Questions.Instance.correctAnswers[quizNumber];
    }

    private void SaveResponsesToFirebase()
    {
        string userId = firebaseManager.UserID;
        int sceneno = firebaseManager.sceneno;

        foreach (var response in responses)
        {
            string key = dbReference.Child("Users").Child(userId).Child("Scene" + sceneno).Push().Key;
            dbReference.Child("Users").Child(userId).Child("fitts" + sceneno).Child(key).SetValueAsync(response);
        }
    }

    private bool IsIdxFingerPinching()
    {
        bool result = false;
        if(rightHand != null){
            result = !wasPinching && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            wasPinching = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        }
        return result;
    }
}
