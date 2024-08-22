using System;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Serialization;

public class FollowInZDirection : MonoBehaviour
{
    public Transform chest;      // The object to follow
    [FormerlySerializedAs("followSpeed")] public float offset; // Speed at which the follower moves
    protected Vector3 originPoint, originForwardVector;
    KalmanFilterVector3 kalmanV3Origin, kalmanV3Forward;
    CircularBuffer.CircularBuffer<Vector3> originHistory, forwardHistory;
    [Header("Kalman Filters")]
    [SerializeField] private int originHistoryWindow = 6;
    [SerializeField] private int forwardHistoryWindow = 6;
    [SerializeField] private float k1q = 0.0001f, k2q = 0.00001f, k1r = 0.1f, k2r = 0.01f;
    private DatabaseReference databaseReference;
    void Start()
    {
        ResetKalmanFilters();
        originPoint = Vector3.zero;
        originForwardVector = Vector3.zero;
        List<string> datafields = new List<string>{"k1q","k1r","k2q","k2r","originWindow","rotationWindow"};
        List<Action<float>> callbacks = new List<Action<float>> { 
            (float newVal) => {k1q = newVal;},
            (float newVal) => {k1r = newVal;},
            (float newVal) => {k2q = newVal;},
            (float newVal) => {k2r = newVal;},
            (float newVal) => {originHistoryWindow = (int) newVal;},
            (float newVal) => {forwardHistoryWindow = (int) newVal;},
        };
        new FirebaseTracking(datafields, callbacks);
        k1q = (float)databaseReference.Child("k1q").GetValueAsync().Result.Value;
        k1r = (float)databaseReference.Child("k1r").GetValueAsync().Result.Value;
        k2q = (float)databaseReference.Child("k2q").GetValueAsync().Result.Value;
        k2r = (float)databaseReference.Child("k2r").GetValueAsync().Result.Value;
        //originHistoryWindow = (int)databaseReference.Child("originWindow").GetValueAsync().Result.Value;
        //rotationHistoryWindow = (int)databaseReference.Child("rotationWindow").GetValueAsync().Result.Value;
    }
    public void ResetKalmanFilters()
    {
        kalmanV3Origin = new KalmanFilterVector3(k1q, k1r);
        kalmanV3Forward = new KalmanFilterVector3(k1q, k1r);
        originHistory = new CircularBuffer.CircularBuffer<Vector3>(originHistoryWindow);
        forwardHistory = new CircularBuffer.CircularBuffer<Vector3>(forwardHistoryWindow);
    }
    void Update()
    {
        // heighten canvas to the head
        // transform.position = new Vector3(transform.position.x, head.position.y, -transform.position.z);
        // transform.rotation = chest.rotation;
        // // rotate the canvas to face the user
        // transform.Rotate(new Vector3 (0,0,90));
        // transform.rotation = Quaternion.LookRotation(transform.position - chest.position);
        Vector3 chestPosition = GetStableCanvasPosition();
        Vector3 chestForward = GetStableCanvasForward();
        transform.SetPositionAndRotation(chestPosition + chestForward.normalized * offset, Quaternion.LookRotation(transform.position - chestPosition));
        // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + offset);
    }
    private Vector3 GetStableCanvasPosition()
    {
        Transform tf = chest.transform;
        if (tf != null)
        {
            originPoint = tf.position;
            originHistory.PushBack(originPoint);
            originPoint = kalmanV3Origin.Update(originHistory.Back(), k1q, k1r);
        }
        return originPoint;
    }
    private Vector3 GetStableCanvasForward()
    {
        Transform tf = chest.transform;
        if (tf != null)
        {
            originForwardVector = tf.up;
            forwardHistory.PushBack(originForwardVector);
            originForwardVector = kalmanV3Forward.Update(forwardHistory.Back(), k1q, k1r);
        }
        return originForwardVector;
    }
}
