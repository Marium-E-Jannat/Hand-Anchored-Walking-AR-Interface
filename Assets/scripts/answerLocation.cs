using TMPro;
using UnityEngine;

public class AnswerLocation : MonoBehaviour
{
    private static AnswerLocation _instance;
    public static AnswerLocation Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AnswerLocation>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<AnswerLocation>();
                    singletonObject.name = typeof(Questions).ToString() + " (Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    public Transform headTransform;
    [SerializeField]
    private GameObject text;
    private bool leftSide = false;
    [SerializeField]
    public int xOffset;
    // Start is called before the first frame update
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
        // Locate the main camera tagged as "MainCamera"
        headTransform = Camera.main != null ? Camera.main.transform : null;

        // Log error if Main Camera is not found
        if (headTransform == null)
        {
            Debug.LogError("Main Camera not found. Ensure your XR Origin has a camera tagged as 'MainCamera'.");
        }
    }
    public void SuspendText(){
        text.SetActive(false);
    }

    public void SetText(string value){
        text.SetActive(true);
        text.GetComponent<TextMeshPro>().text = value.ToString();
        if(leftSide){
            text.transform.SetPositionAndRotation(new Vector3(headTransform.position.x - xOffset, headTransform.position.y, headTransform.position.z), Quaternion.Euler(0, 270, 0));
        }
        else{
            text.transform.SetPositionAndRotation(new Vector3(headTransform.position.x + xOffset, headTransform.position.y, headTransform.position.z), Quaternion.Euler(0, 90, 0));
        }
    }

    public void ResetTxtSide(){
        leftSide = !leftSide;
    }
}
