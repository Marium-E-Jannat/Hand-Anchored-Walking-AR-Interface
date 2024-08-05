using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Events;

public class fittsrecorder2 : MonoBehaviour
{
    public Button[] optionButtons;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    public Color colorAtStill;
    public static int selec;
    public static float startTime;
    [SerializeField] private Button startButton;
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
            status statusCode = CheckForPinch();
            if(statusCode == status.UNPINCH){
                return;
            }
            if(statusCode != status.OUTLIER) {
                if(statusCode == status.ERROR){
                    Debug.Log("Error detected, iteration counted.");
                }else{
                    Debug.Log("Success, iteration counted.");
                }
                ResetButtonColor();
                iteration += 1;
                if (iteration == 10){
                    quizInProgress = false;
                    iteration = 0;
                    quizNumber += 1;
                    if (quizNumber < 8){
                        startNewQuiz.Invoke();
                    }else {
                        SaveResponsesToFirebase();
                        Debug.Log("Quiz complete!");
                        return;
                    }
                }
            }else{
                Debug.Log("Outlier detected, iteration not counted.");
            }
            Debug.Log("Option Selected: " + selec);
            HandleOptionSelected(statusCode);
        }
        // reset iteration, status, metrics
    }

    private void hello()
    {
        Image image = startButton.GetComponent<Image>();
        image.color = colorAtStill;

        // TODO: rewrite to reset only selected button
        for (int i = 0; i < optionButtons.Length; i++)
        {
            Image images = optionButtons[i].GetComponent<Image>();
            images.color = colorAtStill;
            TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            texts.color = Color.black;
        }

        TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
        text.color = Color.black;
    }

    status CheckForPinch()
    {
        if (rightHand != null && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {

            Vector3 pinchPosition = cursor.transform.position;

            // Button button = optionButtons[cindex];
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
        }else{
            return status.UNPINCH;
        }
    }

    // private IEnumerator InitializeQuiz()
    // {
    //     for (qindex = 0; qindex < 8; qindex++)
    //     {
    //         currentQuestionIndex = qindex;
    //         cindex=-1;
    //         yield return StartCoroutine(hello());
    //         // Wait for the right hand pinching to reset
    //         yield return new WaitUntil(()=>rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index) == false);

    //         Image image = startButton.GetComponent<Image>();
    //         image.color = colorAtStill;
    //         TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
    //         text.color = Color.black;

    //         int first = qindex;
    //         int second = (qindex + 4) % 8;
    //         int send = first, t = 0;

    //         for (int i = 0; i < 10; i++)
    //         {

    //             Debug.Log("iteration is " + i);
    //             Debug.Log("send before question shown is " + send);
    //             ShowQuestion(send);
    //             selec = -1;

    //             yield return new WaitUntil(() =>
    //             {
    //                 CheckForPinch(send);
    //                 return selec != -1;
    //             });

    //             if (selec == -3)
    //             {
    //                 Debug.Log("Outlier detected, iteration not counted.");
    //                 i--;
    //                 selec = -1;
    //                 continue;
    //             }
    //             else if (selec == -2)
    //             {
    //                 Debug.Log("Error detected, iteration counted.");
    //                 selec = -1;
    //                 t ^= 1;
    //                 send = (t == 1) ? second : first;
    //                 continue;
    //             }

    //             Debug.Log("Option Selected: " + selec);
    //             yield return StartCoroutine(HandleOptionSelected(selec - 1));

    //             t ^= 1;
    //             send = (t == 1) ? second : first;
    //         }
    //     }
    //     SaveResponsesToFirebase();
    //     Debug.Log("Quiz complete!");
    // }

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
        float timeTaken = Time.time - startTime;
        string statusString;
        if (statusCode == status.ERROR){
            statusString = "Error";
        }else if(statusCode == status.OUTLIER){
            statusString = "Outlier";
        }else{
            statusString = "Success";
        }

        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "Quiz number", quizNumber},
            { "Iteration", iteration},
            { "Time taken", timeTaken },
            { "Status", statusString}
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
