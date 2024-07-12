using UnityEngine;

public class Questions : MonoBehaviour
{
    private static Questions _instance;
    public static Questions Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Questions>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<Questions>();
                    singletonObject.name = typeof(Questions).ToString() + " (Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    public string[] questions;
    public string[,] options;
    public int[] correctAnswers;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        questions = new string[]
        {
            "What is the 20+40?",
            "What is 2 + 2?",
            "What is  7+5?",
            "What is 6+9?",
            "What is the largest planet in our solar system?"
        };

        options = new string[,]
        {
            { "60", "50", "40", "20" },
            { "3", "4", "5", "6" },
            { "11", "10", "1", "12" },
            { "15", "20", "5", "0" },
            { "Earth", "Mars", "Jupiter", "Saturn" }
        };

        correctAnswers = new int[] { 0, 1, 3, 0, 2 };
    }
}
