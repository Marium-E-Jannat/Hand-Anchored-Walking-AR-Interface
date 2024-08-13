using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class CanvasPositionerWithThreshold : MonoBehaviour
{
    [SerializeField] private GameObject controller;

    [SerializeField] private float positionThreshold;

    [SerializeField] private float rotationThreshold;
    [SerializeField] private float speed;

    private DatabaseReference databaseReference;
    private Vector3 newPosition;

    void Start(){
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            ListenForPosThresChange();
            ListForRotThres();
            ListenForSpeedChange();
        });
        newPosition = transform.position;
    }

    void ListForRotThres(){
        databaseReference.Child("Rotation-threshold").ValueChanged += (object sender, ValueChangedEventArgs args) => { 
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot.Exists)
            {
                Debug.Log("rotation threshold from firebase " + args.Snapshot.Value.ToString());
                if(float.TryParse(args.Snapshot.Value.ToString(), out float result))
                {
                    rotationThreshold = result;
                }
            }
        };
    }

    void ListenForSpeedChange(){
        databaseReference.Child("Speed").ValueChanged += (object sender, ValueChangedEventArgs args) => { 
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot.Exists)
            {
                Debug.Log("speed from firebase " + args.Snapshot.Value.ToString());
                if(float.TryParse(args.Snapshot.Value.ToString(), out float result))
                {
                    speed = result;
                }
            }
        };
    }

    void ListenForPosThresChange(){
        databaseReference.Child("Position-threshold").ValueChanged += (object sender, ValueChangedEventArgs args) => { 
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot.Exists)
            {
                Debug.Log("position threshold from firebase " + args.Snapshot.Value.ToString());
                if(float.TryParse(args.Snapshot.Value.ToString(), out float result))
                {
                    positionThreshold = result;
                }
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransformWithController();
    }

    void UpdateTransformWithController() {
        if (controller == null) {
            Debug.Log("No controller was set");
            return;
        }

        Transform ctf = controller.transform;

        if (Vector3.Distance(ctf.position, newPosition) > positionThreshold) {
            newPosition = ctf.position;
        }

        var step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, newPosition, step);

        Quaternion tRot = Quaternion.LookRotation(ctf.position - transform.position);
        float angleDifference = Quaternion.Angle(transform.rotation, tRot);
        if (angleDifference > rotationThreshold) {
            transform.rotation = ctf.rotation;
        }
    }
}
