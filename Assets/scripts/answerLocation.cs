using System;
using System.Collections.Generic;
using Firebase.Database;
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

                    // DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    public Transform headTransform;
    private bool leftSide = false;
    [SerializeField]
    public int xOffset;
    [SerializeField]
    private GameObject leftText, rightText;
    [SerializeField]
    private GameObject leftArrow, rightArrow;
    private Vector3 prevForward;
    private float wallDist;
    private float txtAngle;
    private DatabaseReference databaseReference;
    void Start()
    {
        if (headTransform == null)
        {
            Debug.LogError("Main Camera not found. Ensure your XR Origin has a camera tagged as 'MainCamera'.");
        }
        prevForward = headTransform.forward;

        // set up listen hook to firebase
        List<string> datafields = new List<string>{"Wall-distance", "Text-angle"};
        List<Action<float>> callbacks = new List<Action<float>> { 
            HandleWallDistChanged,
            HandleTxtAngleChanged,
        };
        new FirebaseTracking(datafields, callbacks);
        wallDist = (float)databaseReference.Child("Wall-distance").GetValueAsync().Result.Value;
        txtAngle = (float)databaseReference.Child("Text-angle").GetValueAsync().Result.Value;
        SetTextOnWall();
    }

    private void HandleWallDistChanged(float newVal)
    {
        wallDist = newVal;
        SetTextOnWall();
    }

    private void HandleTxtAngleChanged(float newVal)
    {
        txtAngle = newVal;
        SetTextOnWall();
    }

    private void SetTextOnWall(){
        // leftText.transform.position = new Vector3(-wallDist, leftText.transform.position.y, leftText.transform.position.z);
        // rightText.transform.position = new Vector3(wallDist, rightText.transform.position.y, rightText.transform.position.z);
        var rot = Quaternion.AngleAxis(txtAngle,Vector3.up);
        var negRot = Quaternion.AngleAxis(-txtAngle, Vector3.up);
        // a local direction vector that points in forward direction but also an angle forward.
        // direction in world space you need to transform it.
        float forwardDist = wallDist / Mathf.Sin(txtAngle*Mathf.Deg2Rad);
        leftText.transform.position = transform.TransformDirection(negRot*Vector3.forward).normalized * forwardDist;
        rightText.transform.position = transform.TransformDirection(rot*Vector3.forward).normalized * forwardDist;
    }

    public void SuspendText(){
        // text.SetActive(false);
        leftText.SetActive(false);
        leftArrow.SetActive(false);
        rightText.SetActive(false);
        rightArrow.SetActive(false);
    }

    public void SetText(string value){
        // text.SetActive(true);
        // text.GetComponent<TextMeshPro>().text = value.ToString();
        // if(leftSide){
        //     text.transform.SetPositionAndRotation(new Vector3(headTransform.position.x - xOffset, headTransform.position.y, headTransform.position.z), Quaternion.Euler(0, 270, 0));
        // }
        // else{
        //     text.transform.SetPositionAndRotation(new Vector3(headTransform.position.x + xOffset, headTransform.position.y, headTransform.position.z), Quaternion.Euler(0, 90, 0));
        // }
        if(leftText == null || leftArrow == null || rightText == null || rightArrow == null){
            Debug.LogError("Answer indictors are not set properly.");
        }
        if(leftSide){
            leftText.SetActive(true);
            leftText.GetComponent<TextMeshPro>().text = value.ToString();
            leftArrow.SetActive(true);
        }else{
            rightText.SetActive(true);
            rightText.GetComponent<TextMeshPro>().text = value.ToString();
            rightArrow.SetActive(true);
        }
    }

    void Update(){
        transform.position = headTransform.position;
        // float angle = Vector3.Angle(headTransform.forward, prevForward);
        // if(angle > 90f){
        //     transform.rotation = headTransform.rotation;
        //     prevForward = headTransform.forward;
        // }
    }

    public void ResetTxtSide(){
        leftSide = !leftSide;
    }
}
