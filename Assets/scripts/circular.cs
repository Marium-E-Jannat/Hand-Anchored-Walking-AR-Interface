using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class CircularButtonLayout : MonoBehaviour
{
    public Button startButton;  
    public Button[] buttons;   
    public  static float Radius {get; private set;}
    public static float ButtonRadius {get; private set;}
    private DatabaseReference databaseReference;

    void Start(){

        // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        // {
        //     databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        //     ListenForButtonRadiusChange();
        //     ListenForDistRadiusChange();
        // });
        List<string> datafields = new List<string>{"Distance-radius","Button-radius"};
        List<Action<float>> callbacks = new List<Action<float>> { 
            HandleDistRadiusValueChanged,
            HandleButtonRadiusValueChanged
        };
        new FirebaseTracking(datafields, callbacks);
        Radius = (float)databaseReference.Child("Distance-radius").GetValueAsync().Result.Value;
        ButtonRadius = (float)databaseReference.Child("Button-radius").GetValueAsync().Result.Value;
        HandleLayoutChange();
    }

    private void HandleButtonRadiusValueChanged(float newVal)
    {
                ButtonRadius = newVal;
                HandleLayoutChange();
    }

    private void HandleDistRadiusValueChanged(float newVal)
    {
                Radius = newVal;
                HandleLayoutChange();
    }

    void HandleLayoutChange()
    {
        
        startButton.transform.localPosition = new Vector3(0,0,-2);

        RectTransform startButtonRect = startButton.GetComponent<RectTransform>();
        startButtonRect.sizeDelta = new Vector2(ButtonRadius * 2, ButtonRadius * 2);
        float angleStep = 360f / buttons.Length;
        float angle = 0f;

       
        for (int i = 0; i < buttons.Length; i++)
        {
            
            float x = (Radius+ButtonRadius) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = (Radius+ButtonRadius) * Mathf.Sin(angle * Mathf.Deg2Rad);

            
            buttons[i].transform.localPosition = new Vector3(x, y, -2);
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(ButtonRadius * 2, ButtonRadius * 2);

           
            angle += angleStep;
        }
    }
}
