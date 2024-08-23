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
        // unused for now
        questions = new string[]
        {
            "x=0, x+3=?",
            "x=2, x+5=?",
            "x=6, x+7=?",
            "x=4, x+2=?",
            "x=look, x+3=?",
            "x=look, x+4=?",
            "x=look, x+5=?",
            "x=look, x+6=?"
        };

        options = new string[,]
        {
            { "3", "2", "5", "6" },
            { "2", "7", "5", "6" },
            { "15", "1", "10", "3" },
            { "6", "2", "5", "0" },
            { "4", "2", "7", "0" },
            { "9", "2", "6", "0" },
            { "6", "2", "5", "8" },
            { "7", "0", "2", "10" },
        };

        correctAnswers = new int[] {0,1,3,0,0,2,3,1};
    }
}
