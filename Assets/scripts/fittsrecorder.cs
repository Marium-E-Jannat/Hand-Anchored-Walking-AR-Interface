using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class fittsrecorder : MonoBehaviour
{
    public  Button[] optionButtons;
    public Color genericColor2;
    public static bool firsel = true;
    public static int selec;
    public static int btnsave = -1;
    private PanelManager panelManager;
    public int cindex;
    private int currentQuestionIndex = 0;
    private float startTime;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();
    private int qindex;
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
            for(int i=0;i<8;i++)
            {
                Image images = optionButtons[i].GetComponent<Image>();
                images.color=genericColor2;
                TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                texts.color=Color.black;
            }
            TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color=Color.black;
            selec=-1; 
            yield return new WaitUntil(() => selec == 0);

    }
    private IEnumerator InitializeQuiz()
    {

        for (qindex = 0; qindex < 8; qindex++)
        {
            currentQuestionIndex = qindex;
            yield return StartCoroutine(hello());
            Debug.Log("hello welcome you from start");
            Image image = startButton.GetComponent<Image>();
            image.color=genericColor2;
            TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color=Color.black;
            int first=qindex;
            int second=qindex+4;
            if(qindex>=4){
                first=qindex;
                second=qindex-4;
            }
            int send=first,t=0;
            selec=-1;
            for(int i=0;i<10;i++)
            {
                Debug.Log("the random dot index "+qindex);
                ShowQuestion(send);
                yield return new WaitUntil(() => selec != -1);
                Debug.Log("Option Selected"+selec);
                Debug.Log("hello");
                yield return StartCoroutine(HandleOptionSelected(selec-1));
                t^=1;
                if(t==1){send=second;}
                else{send=first;}
            }
            
        }
        SaveResponsesToFirebase();
        Debug.Log("Quiz complete!");
    }
public  void ShowQuestion(int send)
{
     
    Debug.Log("hello from showquestion"+send);
    startTime = Time.time;
    cindex=send;
    EyeTracking.PinchCounter=0;
    EyeTracking.distance=0;
    for (int i = 0; i < 4; i++)
    {
        optionButtons[i].interactable = true;
        Image images = optionButtons[i].GetComponent<Image>();
        images.color=genericColor2;
        TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        texts.color=Color.black;

    }
    Image image = optionButtons[cindex].GetComponent<Image>();
    image.color=Color.red;
}

    public IEnumerator HandleOptionSelected(int optionIndex)
    {
        float timeTaken = Time.time - startTime; 
        bool isCorrect = optionIndex == cindex;
        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "random asker no ", currentQuestionIndex + 1 }, 
            { "responseCorrect", isCorrect },
            { "timeTaken", timeTaken },
            {"Pinches",EyeTracking.PinchCounter},
            {"Distance from centre of button",EyeTracking.distance}
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

    private ColorBlock ResetColorBlock(Color gene)
    {
        Debug.Log("reset color block called ");
        ColorBlock cb=optionButtons[0].colors;
        cb.normalColor =gene;
        cb.selectedColor =gene;
        return cb;
    }
}
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class fittsrecorder : MonoBehaviour
{
    public  Button[] optionButtons;
    public Color genericColor2;
    public static bool firsel = true;
    public static int selec;
    public static int btnsave = -1;
    private PanelManager panelManager;
    public int cindex;
    private int currentQuestionIndex = 0;
    private float startTime;
    private DatabaseReference dbReference;
    private FirebaseSceneManager firebaseManager;
    private List<Dictionary<string, object>> responses = new List<Dictionary<string, object>>();
    private int qindex;
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
            for(int i=0;i<8;i++)
            {
                Image images = optionButtons[i].GetComponent<Image>();
                images.color=genericColor2;
                TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                texts.color=Color.black;
            }
            TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color=Color.black;
            selec=-1; 
            yield return new WaitUntil(() => selec == 0);

    }
    private IEnumerator InitializeQuiz()
    {

        for (qindex = 0; qindex < 4; qindex++)
        {
            currentQuestionIndex = qindex;
            yield return StartCoroutine(hello());
            Debug.Log("hello welcome you from start");
            Image image = startButton.GetComponent<Image>();
            image.color=genericColor2;
            TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color=Color.black;
            selec=-1;
            Debug.Log("the random dot index "+qindex);
            ShowQuestion();
            yield return new WaitUntil(() => selec != -1);
            Debug.Log("Option Selected"+selec);
            Debug.Log("hello");
            yield return StartCoroutine(HandleOptionSelected(selec-1));
            
        }
        /*for(int i=0;i<8;i++)
        {
            Button button=optionButtons[i].GetComponent<Button>;
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.sizeDelta=150f;
        }
        CircularButtonLayout.buttonRadius=100;
        for (qindex = 4; qindex < 8; qindex++)
        {
             currentQuestionIndex = qindex;
            yield return StartCoroutine(hello());
            Image image = startButton.GetComponent<Image>();
            image.color=genericColor2;
            TextMeshProUGUI text = startButton.GetComponentInChildren<TextMeshProUGUI>();
            text.color=Color.black;
            selec=-1;
            Debug.Log("the random dot index "+qindex);
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
    System.Random random=new System.Random();
    cindex=random.Next(0,8);
    for (int i = 0; i < 4; i++)
    {
        optionButtons[i].interactable = true;
        Image images = optionButtons[i].GetComponent<Image>();
        images.color=genericColor2;
        TextMeshProUGUI texts = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        texts.color=Color.black;

    }
    Image image = optionButtons[cindex].GetComponent<Image>();
    image.color=Color.red;
}

    public IEnumerator HandleOptionSelected(int optionIndex)
    {
        float timeTaken = Time.time - startTime; 
        bool isCorrect = optionIndex == cindex;
        Dictionary<string, object> response = new Dictionary<string, object>
        {
            { "random asker no ", currentQuestionIndex + 1 }, 
            { "responseCorrect", isCorrect },
            { "timeTaken", timeTaken },
            {"Pinches",EyeTracking.PinchCounter},
            {"Distance from centre of button",EyeTracking.distance}
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

    private ColorBlock ResetColorBlock(Color gene)
    {
        Debug.Log("reset color block called ");
        ColorBlock cb=optionButtons[0].colors;
        cb.normalColor =gene;
        cb.selectedColor =gene;
        return cb;
    }
}
*/