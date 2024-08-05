using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class FirebaseLayoutManager : MonoBehaviour
{
    public static FirebaseLayoutManager Instance { get; private set; }
    private DatabaseReference databaseReference;
    public UnityEvent<float> updateRadius = new UnityEvent<float>();
    public UnityEvent<float> updateDistance = new UnityEvent<float>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Initialize Firebase");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                app.Options.DatabaseUrl = new System.Uri("https://trackingtech-e08ef-default-rtdb.firebaseio.com");
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("connected success to firebase");

                ListenForButtonRadiusChange();
                ListenForDistRadiusChange();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
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
