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
    public static bool quizInProgress;
    int quizNumber = 0;
    int optionNumber;
    int iteration = 0;
    public UnityEvent startNewQuiz = new UnityEvent();
    [SerializeField] private GameObject startPanel; 
    [SerializeField] private GameObject questionPanel; 
    private Button prevButtonClicked;

    enum status {
        ERROR,
        SUCCESS
    }
    bool wasPinching = false;
    public float distanceUpperThreshold = 0.2f, distanceLowerThreshold = 0.05f;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();
    private int correctAnswer;
    private int pickedAnswer;
    [SerializeField] private Button submitButton;

    void Start()
    {
        firebaseManager = FirebaseSceneManager.Instance;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
        colorAtStill = optionButtons[1].colors.normalColor;
        quizInProgress = false;
        questionPanel.SetActive(false);
        startPanel.SetActive(false);
    }

    private void Update(){
        if (quizInProgress){
            startPanel.SetActive(false);
            questionPanel.SetActive(true);
            // if(iteration%2==0){
            //     optionNumber = quizNumber;
            // }else{
            //     optionNumber = (quizNumber + 4) % 8;
            // }
            ShowQuestion();
            // if(wasPinching && !rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index)) {
            //     status statusCode = CheckForSubmitPinch();
            //     HandleOptionSelected(statusCode);

            //     if (statusCode != status.OUTLIER)
            //     {
            //         if (statusCode == status.ERROR)
            //         {
            //             Debug.Log("Error detected, iteration counted.");
            //         }
            //         else
            //         {
            //             Debug.Log("Success, iteration counted.");
            //         }
            //         ResetButtonColor();
            //         iteration += 1;
            //         // reset timer for new iteration
            //         startTime = DateTime.Now;
            //         if (iteration == 4)
            //         {
            //             quizInProgress = false;
            //             iteration = 0;
            //             quizNumber += 1;
            //             if (quizNumber < 4)
            //             {
            //                 startNewQuiz.Invoke();
            //             }
            //             else
            //             {
            //                 SaveResponsesToFirebase();
            //                 Debug.Log("Quiz complete!");
            //                 return;
            //             }
            //         }
            //     }
            //     else
            //     {
            //         // restart counter for outlier
            //         startTime = DateTime.Now;
            //         Debug.Log("Outlier detected, iteration not counted.");
            //     }
            // }
        } else{
            startPanel.SetActive(true);
            questionPanel.SetActive(false);
        }
        // wasPinching = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        // reset iteration, status, metrics
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
        if(prevButtonClicked != null){
            ResetButtonColor(prevButtonClicked);
        }
        quizInProgress = true;
        SetButtonColor(button, Color.green);
        startTime = DateTime.Now;
        prevButtonClicked = button;
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
    }

    public void OnSubmitClicked(Button button){
        if(prevButtonClicked != null){
            ResetButtonColor(prevButtonClicked);
        }
        double timeTaken = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        string statusString;
        if (pickedAnswer != correctAnswer){
            statusString = "Error";
        }else{
            statusString = "Success";
        }

        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "Quiz number", quizNumber},
            { "Time taken", timeTaken },
            { "Status", statusString},
        };

        responses.Add(response);
        if(quizNumber == Questions.Instance.questions.Length - 1){
            SaveResponsesToFirebase();
        }else{
            quizNumber += 1;
            quizInProgress = false;
        }
        prevButtonClicked = button;
    }

    // bool CheckForSubmitPinch()
    // {
    //     if(selec == 5){
    //         return true;
    //     }
    //     if(selec < 5){
    //         pickedAnswer = selec;
    //         submitButton.interactable = true;
    //     }
    //     return false;
    // }

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

        questionText.text = question;
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

    // private void ResetButtonColor()
    // {
    //     Image image = optionButtons[optionNumber].GetComponent<Image>();
    //     if (image != null)
    //     {
    //         image.color = colorAtStill;
    //     }
    // }

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
}
