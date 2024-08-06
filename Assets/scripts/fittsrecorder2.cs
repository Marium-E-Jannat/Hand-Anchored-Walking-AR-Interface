using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Events;
using System;

public class fittsrecorder2 : MonoBehaviour
{
    public Button[] optionButtons;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    public Color colorAtStill;
    public static int selec;
    public static DateTime startTime;
    [SerializeField] private OVRHand rightHand;
    public static bool quizInProgress = false;
    int quizNumber = 0;
    int optionNumber;
    int iteration = 0;
    public UnityEvent startNewQuiz = new UnityEvent();
    public GameObject cursor;

    enum status {
        ERROR,
        SUCCESS,
        OUTLIER,
        UNPINCH
    }
    bool wasPinching = false;
    public float distanceUpperThreshold = 0.2f, distanceLowerThreshold = 0.05f;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();

    void Start()
    {
        firebaseManager = FirebaseSceneManager.Instance;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
        colorAtStill = optionButtons[1].colors.normalColor;
    }

    private void Update(){
        if (quizInProgress){
            if(iteration%2==0){
                optionNumber = quizNumber;
            }else{
                optionNumber = (quizNumber + 4) % 8;
            }
            ShowQuestion();
            if(wasPinching && !rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index)) {
                status statusCode = CheckForPinch();
                HandleOptionSelected(statusCode);

                if (statusCode != status.OUTLIER)
                {
                    if (statusCode == status.ERROR)
                    {
                        Debug.Log("Error detected, iteration counted.");
                    }
                    else
                    {
                        Debug.Log("Success, iteration counted.");
                    }
                    ResetButtonColor();
                    iteration += 1;
                    // reset timer for new iteration
                    startTime = DateTime.Now;
                    if (iteration == 10)
                    {
                        quizInProgress = false;
                        iteration = 0;
                        quizNumber += 1;
                        if (quizNumber < 8)
                        {
                            startNewQuiz.Invoke();
                        }
                        else
                        {
                            SaveResponsesToFirebase();
                            Debug.Log("Quiz complete!");
                            return;
                        }
                    }
                }
                else
                {
                    // restart counter for outlier
                    startTime = DateTime.Now;
                    Debug.Log("Outlier detected, iteration not counted.");
                }
            }
        }
        wasPinching = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        // reset iteration, status, metrics
    }

    status CheckForPinch()
    {
        Vector3 pinchPosition = cursor.transform.position;
        Button button = optionButtons[optionNumber];
        Debug.Log("current option number is " + optionNumber);
        Vector3 buttonPosition = button.transform.position;
        float distance = Vector2.Distance(
            new Vector2(pinchPosition.x, pinchPosition.y),
            new Vector2(buttonPosition.x, buttonPosition.y)
        );

        if (distance > distanceLowerThreshold && distance <= distanceUpperThreshold)
        {
            return status.ERROR;
        }
        else if(distance > distanceUpperThreshold)
        {
            return status.OUTLIER;
        }else{
            return status.SUCCESS;
        }
    }

    public void ShowQuestion()
    {
        Debug.Log("Showing question at index: " + optionNumber);
        Image image = optionButtons[optionNumber].GetComponent<Image>();
        image.color = Color.red;
    }

    private void ResetButtonColor()
    {
        Image image = optionButtons[optionNumber].GetComponent<Image>();
        if (image != null)
        {
            image.color = colorAtStill;
        }
    }

    private void HandleOptionSelected(status statusCode)
    {
        double timeTaken = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        string statusString;
        if (statusCode == status.ERROR){
            statusString = "Error";
        }else if(statusCode == status.OUTLIER){
            statusString = "Outlier";
        }else{
            statusString = "Success";
        }

        // Debug.Log("Option Selected: " + selec);
        // Debug.Log("Quiz " + quizNumber);
        // Debug.Log("Iteration "+iteration);
        // Debug.Log("Time " + timeTaken);

        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "Quiz number", quizNumber},
            { "Iteration", iteration},
            { "Time taken", timeTaken },
            { "Status", statusString},
            { "Button radius", CircularButtonLayout.ButtonRadius},
            { "Distance radius", CircularButtonLayout.Radius},
        };

        responses.Add(response);
        selec = -1;
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
}
