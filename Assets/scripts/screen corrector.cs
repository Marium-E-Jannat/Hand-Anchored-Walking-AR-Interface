using UnityEngine;
using UnityEngine.Serialization;

public class FollowInZDirection : MonoBehaviour
{
    public Transform target;      // The object to follow
    [FormerlySerializedAs("followSpeed")] public float offset = 5f; // Speed at which the follower moves

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z) + new Vector3(0,0,offset);
    }
    

}
