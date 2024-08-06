using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class CircularButtonLayout : MonoBehaviour
{
    public Button startButton;  
    public Button[] buttons;   
    private float radius = 100f; 
    private float buttonRadius = 50f;
    private DatabaseReference databaseReference;

    void Start(){

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            ListenForButtonRadiusChange();
            ListenForDistRadiusChange();
        });
        radius = (float)databaseReference.Child("Distance-radius").GetValueAsync().Result.Value;
        buttonRadius = (float)databaseReference.Child("Button-radius").GetValueAsync().Result.Value;
        HandleLayoutChange();
    }

    private void ListenForButtonRadiusChange()
    {
        databaseReference.Child("Button-radius").ValueChanged += HandleButtonRadiusValueChanged;
    }

    private void HandleButtonRadiusValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            Debug.Log("button radius from firebase " + args.Snapshot.Value.ToString());
            if(float.TryParse(args.Snapshot.Value.ToString(), out float result))
            {
                buttonRadius = result;
                HandleLayoutChange();
            }
        }
    }

    private void ListenForDistRadiusChange()
    {
        databaseReference.Child("Distance-radius").ValueChanged += HandleDistRadiusValueChanged;
    }

    private void HandleDistRadiusValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            Debug.Log("button dist radius from firebase " + args.Snapshot.Value.ToString());
            if (float.TryParse(args.Snapshot.Value.ToString(), out float result))
            {
                radius = result;
                HandleLayoutChange();
            }
        }
    }

    void HandleLayoutChange()
    {
        
        startButton.transform.localPosition = new Vector3(0,0,-1);

        RectTransform startButtonRect = startButton.GetComponent<RectTransform>();
        startButtonRect.sizeDelta = new Vector2(buttonRadius * 2, buttonRadius * 2);
        float angleStep = 360f / buttons.Length;
        float angle = 0f;

       
        for (int i = 0; i < buttons.Length; i++)
        {
            
            float x = (radius+buttonRadius) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = (radius+buttonRadius) * Mathf.Sin(angle * Mathf.Deg2Rad);

            
            buttons[i].transform.localPosition = new Vector3(x, y, -1);
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonRadius * 2, buttonRadius * 2);

           
            angle += angleStep;
        }
    }
}
