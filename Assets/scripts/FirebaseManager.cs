using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class FirebaseSceneManager : MonoBehaviour
{
    public static FirebaseSceneManager Instance { get; private set; }
    public string sceneName;
    private DatabaseReference databaseReference;

    void Update()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make the GameObject persistent across scenes
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("intialize firebase");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                app.Options.DatabaseUrl = new System.Uri("https://trackingtech-e08ef-default-rtdb.firebaseio.com");
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                ListenForSceneChange();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    private void ListenForSceneChange()
    {
        databaseReference.Child("Scene").Child("name").ValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            long sceneNo = (long)args.Snapshot.Value; // Cast to long
            ChangeScene((int)sceneNo); // Cast to int for ChangeScene
        }
    }

    private void ChangeScene(int sceneNo)
    {
        switch (sceneNo)
        {
            case 0:
                sceneName = "scene manager";
                Debug.Log("scene 0");
                break;
            case 1:
                sceneName = "fixed screen";
                Debug.Log("scene 1");
                break;
            case 2:
                sceneName = "head_track_screen";
                Debug.Log("scene 2");
                break;
            case 3:
                sceneName = "move with person";
                Debug.Log("scene 3");
                break;
            case 4:
                sceneName = "project on palm";
                Debug.Log("scene 4");
                break;
            case 5:
                sceneName = "up from palm";
                Debug.Log("scene 5");
                break;
            case 6:
                sceneName = "wrist";
                Debug.Log("scene 6");
                break;
            case 7:
                sceneName = "pinch";
                Debug.Log("scene 7");
                break;
            case 8:
                sceneName = "pinch and move";
                Debug.Log("scene 8");
                break;
            // Add more cases as needed
        }
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " cannot be loaded.");
        }
    }
}

 