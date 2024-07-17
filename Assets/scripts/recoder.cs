using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class Recorder : MonoBehaviour
{
    public TMP_Text questionText;
    public  Button[] optionButtons;
    public Color genericColor2;
    public static bool firsel = true;
    public static int selec = -1;
    public static int btnsave = -1;
    private PanelManager panelManager;

    private int currentQuestionIndex = 0;
    private float startTime;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();
    private int qindex;
    [SerializeField] private GameObject startPanel; 
    [SerializeField] private GameObject questionPanel; 
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
        genericColor2=optionButtons[1].colors.normalColor;
    }
    private IEnumerator hello()
    {
         Image image = startButton.GetComponent<Image>();
            image.color=genericColor2;
             TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color=Color.black;
            selec=-1;
            startPanel.SetActive(true); 
            questionPanel.SetActive(false); 
            yield return new WaitUntil(() => selec != -1);
    }
    private IEnumerator InitializeQuiz()
    {
        if (Questions.Instance == null)
        {
            Debug.LogError("Questions.Instance is null");
            yield break;
        }

        for (qindex = 0; qindex < 8; qindex++)
        {
            currentQuestionIndex = qindex;
            yield return StartCoroutine(hello());
            startPanel.SetActive(false);
            questionPanel.SetActive(true);
            selec=-1;
            Debug.Log("QuestionIndex "+qindex);
            ShowQuestion();
            yield return new WaitUntil(() => selec != -1);
            Debug.Log("Option Selected"+selec);
            Debug.Log("hello");
            yield return StartCoroutine(HandleOptionSelected(selec-1));
            
        }
        
        SaveResponsesToFirebase();
        Debug.Log("Quiz complete!");
    }
public  void ShowQuestion()
{
     
    Debug.Log("hello from showquestion"+qindex);
    startTime = Time.time;
    EyeTracking.PinchCounter=0;
    EyeTracking.distance=0;
    if (Questions.Instance == null || Questions.Instance.questions == null || Questions.Instance.questions.Length == 0)
    {
        Debug.LogError("Questions.Instance or its questions are null or empty.");
        return;
    }
    string question = Questions.Instance.questions[qindex];
    string[] options = new string[4];
    for (int i = 0; i < 4; i++)
    {
        options[i] = Questions.Instance.options[qindex, i];
    }

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

        buttonText.text = options[i];
        optionButtons[i].interactable = true;
         Image image = optionButtons[i].GetComponent<Image>();
         image.color=genericColor2;
        TextMeshProUGUI text = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        text.color=Color.black;

    }
     Image images = optionButtons[4].GetComponent<Image>();
         images.color=genericColor2;
        TextMeshProUGUI texts = optionButtons[4].GetComponentInChildren<TextMeshProUGUI>();
        texts.color=Color.black;
}

   /* public void anotherHit()
    {
         for (int i = 0; i < 4; i++)
    {
        if (optionButtons[i] == null)
        {
            Debug.LogError($"optionButtons[{i}] is not assigned.");
            continue;
        }
         Image image = optionButtons[i].GetComponent<Image>();
         image.color=genericColor2;
        TextMeshProUGUI text = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        text.color=Color.black;

    }
    }*/
    public IEnumerator HandleOptionSelected(int optionIndex)
    {
        
        selec = -1;
        yield return new WaitUntil(() => selec == 5);
        optionIndex=btnsave;
        Debug.Log("hey the last is "+optionIndex);
        float timeTaken = Time.time - startTime; 
        bool isCorrect = optionIndex == Questions.Instance.correctAnswers[currentQuestionIndex];
        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "questionNumber", currentQuestionIndex + 1 }, 
            { "responseCorrect", isCorrect },
            { "timeTaken", timeTaken },
            {"Pinches",EyeTracking.PinchCounter},
            {"Distance from centre of button",EyeTracking.distance}
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
            dbReference.Child("Users").Child(userId).Child("Scene" + sceneno).Child(key).SetValueAsync(response);
        }
    }

    private ColorBlock ResetColorBlock(Color gene)
    {
        Debug.Log("reset color block called ");
        ColorBlock cb=optionButtons[0].colors;
        cb.normalColor =gene;
        cb.selectedColor =gene;
        return cb;
    }
}