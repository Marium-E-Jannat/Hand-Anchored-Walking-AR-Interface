using System;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class CanvasPositionerWithKalman : MonoBehaviour
{
    protected Vector3 originPoint, originRotationVector;
    KalmanFilterVector3 kalmanV3Origin, kalmanV3Rotation;
    CircularBuffer.CircularBuffer<Vector3> originHistory, rotationHistory;
    [Header("Kalman Filters")]
    [SerializeField] private int originHistoryWindow = 6;
    [SerializeField] private int rotationHistoryWindow = 10;
    [SerializeField] private float k1q = 0.0001f, k2q = 0.00001f, k1r = 0.1f, k2r = 0.01f;
    // [SerializeField] private float k1qEyes = 0.0001f, k2qEyes = 0.00001f, k1rEyes = 0.1f, k2rEyes = 0.000001f;
    [SerializeField] private GameObject controller;
    private DatabaseReference databaseReference;
    //bool dirxold = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetKalmanFilters();
        originPoint = Vector3.zero;
        originRotationVector = Vector3.zero;
        List<string> datafields = new List<string>{"k1q","k1r","k2q","k2r","originWindow","rotationWindow"};
        List<Action<float>> callbacks = new List<Action<float>> { 
            (float newVal) => {k1q = newVal;},
            (float newVal) => {k1r = newVal;},
            (float newVal) => {k2q = newVal;},
            (float newVal) => {k2r = newVal;},
            (float newVal) => {originHistoryWindow = (int) newVal;},
            (float newVal) => {rotationHistoryWindow = (int) newVal;},
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
        kalmanV3Rotation = new KalmanFilterVector3(k2q, k2r);
        // kalmanV3Origin.Reset();
        // kalmanV3Rotation.Reset();
        // kalmanV3Rotation.x = transform.rotation.eulerAngles;

        originHistory = new CircularBuffer.CircularBuffer<Vector3>(originHistoryWindow);
        rotationHistory = new CircularBuffer.CircularBuffer<Vector3>(rotationHistoryWindow);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.SetPositionAndRotation(GetStableCanvasPosition(), controller.transform.rotation);
        transform.SetPositionAndRotation(GetStableCanvasPosition(), Quaternion.Euler(GetStableCanvasOrientation()));
    }

    private Vector3 GetStableCanvasPosition()
    {
        Transform tf = controller.transform;
        if (tf != null)
        {
            originPoint = tf.position;
            originHistory.PushBack(originPoint);
            originPoint = kalmanV3Origin.Update(originHistory.Back(), k1q, k1r);
        }
        return originPoint;
    }

    private Vector3 GetStableCanvasOrientation()
    {
        Transform tf = controller.transform;
        //bool dirx = false;
        //bool diry = false;
        //bool dirz = false;
        if (tf != null)
        {
            originRotationVector = tf.rotation.eulerAngles;
            originRotationVector = new Vector3((originRotationVector.x + 180) % 360, (originRotationVector.y + 180) % 360, originRotationVector.z);
            // if(originRotationVector.x > 180) {
            //     dirx = true;
            //     originRotationVector = new Vector3(360 - originRotationVector.x, originRotationVector.y, originRotationVector.z);
            // }

            // if(originRotationVector.x > 5 && originRotationVector.x < 175) {
            //     dirxold = dirx;
            // }
            Debug.Log("origin rotation vector 1" + originRotationVector);
            rotationHistory.PushBack(originRotationVector);

            originRotationVector = kalmanV3Rotation.Update(rotationHistory.Back(), k2q, k2r);
            originRotationVector = new Vector3((originRotationVector.x - 180) % 360, (originRotationVector.y - 180) % 360, originRotationVector.z);
            // Debug.Log("origin rotation vector 2" + originRotationVector);
            // if(dirxold) originRotationVector = new Vector3(360 - originRotationVector.x, originRotationVector.y, originRotationVector.z);
        }
        return originRotationVector;
    }
}
