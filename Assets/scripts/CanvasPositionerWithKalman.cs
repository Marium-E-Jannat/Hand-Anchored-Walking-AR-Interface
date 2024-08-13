using UnityEngine;

public class CanvasPositionerWithKalman : MonoBehaviour
{
    protected Vector3 originPoint, originRotationVector;
    KalmanFilterVector3 kalmanV3Origin, kalmanV3Rotation;
    CircularBuffer.CircularBuffer<Vector3> originHistory, rotationHistory;
    [Header("Kalman Filters")]
    [SerializeField] private int originHistoryWindow = 4;
    [SerializeField] private int rotationHistoryWindow = 6;
    [SerializeField] private float k1q = 0.0001f, k2q = 0.0001f, k1r = 0.1f, k2r = 0.1f;
    [SerializeField] private float k1qEyes = 0.0001f, k2qEyes = 0.00001f, k1rEyes = 0.1f, k2rEyes = 0.000001f;
    [SerializeField] private GameObject controller;

    // Start is called before the first frame update
    void Start()
    {
        ResetKalmanFilters();
        originPoint = Vector3.zero;
        originRotationVector = Vector3.zero;
    }

    public void ResetKalmanFilters()
    {
        kalmanV3Origin = new KalmanFilterVector3(k1q, k1r);
        kalmanV3Rotation = new KalmanFilterVector3(k2q, k2r);

        originHistory = new CircularBuffer.CircularBuffer<Vector3>(originHistoryWindow);
        rotationHistory = new CircularBuffer.CircularBuffer<Vector3>(rotationHistoryWindow);
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(GetStableCanvasPosition(), controller.transform.rotation);
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
        if (tf != null)
        {
            originRotationVector = tf.rotation.eulerAngles;
            rotationHistory.PushBack(originRotationVector);
            originRotationVector = kalmanV3Rotation.Update(rotationHistory.Back(), k2q, k2r);
        }
        return originRotationVector;
    }
}
