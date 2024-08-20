using UnityEngine;
using UnityEngine.Serialization;

public class FollowInZDirection : MonoBehaviour
{
    public Transform chest;      // The object to follow
    [FormerlySerializedAs("followSpeed")] public float offset; // Speed at which the follower moves

    void Update()
    {
        // heighten canvas to the head
        // transform.position = new Vector3(transform.position.x, head.position.y, -transform.position.z);
        // transform.rotation = chest.rotation;
        // // rotate the canvas to face the user
        // transform.Rotate(new Vector3 (0,0,90));
        // transform.rotation = Quaternion.LookRotation(transform.position - chest.position);
        transform.SetPositionAndRotation(chest.transform.position + chest.transform.up.normalized * offset, Quaternion.LookRotation(transform.position - chest.position));
        // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + offset);
    }
}
