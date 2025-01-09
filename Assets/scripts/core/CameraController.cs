using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 5f; 
    public Vector3 offset; 

    private void Update()
    {
        transform.position = new Vector3(target.position.x,target.position.y,transform.position.z);
    }
}