using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class fittsrecorder : MonoBehaviour
{
    public Button[] optionButtons;
    //public Button largeUglyButton;
    public GameObject cursor;
    public int pinchCount = 0;
    private bool isPinching = false;
    public Color genericColor2;
    public static bool firsel = true;
    public static int selec;
    public static int btnsave = -1;
    private PanelManager panelManager;
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private Canvas canvas;
    public float distanceUpperThreshold = 0.2f, distanceLowerThreshold = 0.05f;
    public int cindex = -1;
    private int currentQuestionIndex = 0;
    private float startTime;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();
    private int qindex;
    private int outlier = 0;
    private int error = 0;
    [SerializeField] private Button startButton;

    void Start()
    {
        firebaseManager = FirebaseSceneManager.Instance;
        qindex = 0;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            StartCoroutine(InitializeQuiz());
        });

        genericColor2 = optionButtons[1].colors.normalColor;
    }

    private IEnumerator hello()
    {
        Image image = startButton.GetComponent<Image>();
        image.color = genericColor2;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            Image images = optionButtons[i].GetComponent<Image>();
            images.color = genericColor2;
            TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            texts.color = Color.black;
        }

        TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
        text.color = Color.black;
        selec = -1;
        yield return new WaitUntil(() => selec == 0);
    }

    void CheckForPinch()
    {
        if (rightHand != null)
        {
            bool pinchDetected = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

            if (pinchDetected)
            {
                if (selec == -1)
                {
                    if (!isPinching)
                    {
                        isPinching = true;
                        pinchCount++;

                        Vector3 pinchPosition = cursor.transform.position;

                        Button button = optionButtons[cindex];
                        Vector3 buttonPosition = button.transform.position;
                        float distance = Vector2.Distance(
                            new Vector2(pinchPosition.x, pinchPosition.y),
                            new Vector2(buttonPosition.x, buttonPosition.y)
                        );

                        if (distance > distanceLowerThreshold && distance <= distanceUpperThreshold)
                        {
                            error++;
                            selec = -2; 
                        }
                        else if(distance > distanceUpperThreshold)
                        {
                            outlier++;
                            selec = -3; 
                        }
                    }
                }
            }
            else
            {
                isPinching = false; // Reset the flag when no longer pinching
            }
        }
    }

    private IEnumerator InitializeQuiz()
    {
        for (qindex = 0; qindex < 8; qindex++)
        {
            currentQuestionIndex = qindex;
            cindex=-1;
            yield return StartCoroutine(hello());

            Image image = startButton.GetComponent<Image>();
            image.color = genericColor2;
            TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color = Color.black;

            int first = qindex;
            int second = (qindex + 4) % 8;
            int send = first, t = 0;

            for (int i = 0; i < 10; i++)
            {
                ShowQuestion(send);
                selec = -1;

                yield return new WaitUntil(() =>
                {
                    CheckForPinch();
                    return selec != -1;
                });

                if (selec == -3)
                {
                    Debug.Log("Outlier detected, iteration not counted.");
                    i--;
                    selec = -1;
                    continue;
                }
                else if (selec == -2)
                {
                    Debug.Log("Error detected, iteration counted.");
                    selec = -1;
                    continue;
                }

                Debug.Log("Option Selected: " + selec);
                yield return StartCoroutine(HandleOptionSelected(selec - 1));

                t ^= 1;
                send = (t == 1) ? second : first;
            }
        }
        SaveResponsesToFirebase();
        Debug.Log("Quiz complete!");
    }

    public void ShowQuestion(int send)
    {
        Debug.Log("Showing question at index: " + send);
        startTime = Time.time;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].interactable = true;
            Image images = optionButtons[i].GetComponent<Image>();
            images.color = genericColor2;
            TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            texts.color = Color.black;
        }

        cindex = send;
        Image image = optionButtons[cindex].GetComponent<Image>();
        image.color = Color.red;

        //optionButtons[8].transform.position = optionButtons[cindex].transform.position;
    }

    public IEnumerator HandleOptionSelected(int optionIndex)
    {
        float timeTaken = Time.time - startTime;
        bool isCorrect = optionIndex == cindex;

        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "random asker no", currentQuestionIndex + 1 },
            { "responseCorrect", isCorrect },
            { "timeTaken", timeTaken },
            { "Pinches", pinchCount },
            //{ "Distance from centre of button", distance },
            { "No of errors", error },
            { "No of outliers", outlier }
        };

        responses.Add(response);
        selec = -1;
        yield return null;
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
