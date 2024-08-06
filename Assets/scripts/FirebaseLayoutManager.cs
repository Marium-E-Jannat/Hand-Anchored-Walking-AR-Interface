using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Events;

public class FirebaseLayoutManager : MonoBehaviour
{
    public static FirebaseLayoutManager Instance { get; private set; }
    private DatabaseReference databaseReference;
    private FirebaseSceneManager firebaseManager;
    public UnityEvent<float> updateRadius = new UnityEvent<float>();
    public UnityEvent<float> updateDistance = new UnityEvent<float>();
    void Start(){
        firebaseManager = FirebaseSceneManager.Instance;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            ListenForButtonRadiusChange();
            ListenForDistRadiusChange();
        });
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
                updateRadius.Invoke(result);
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
                updateDistance.Invoke(result);
            }
        }
    }
}
