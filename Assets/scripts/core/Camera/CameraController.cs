using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset;
    public float defaultSize;
    public float minX ;
    public float maxX ;
    public float minY ;
    public float maxY ;

    private float camHalfHeight;
    private float camHalfWidth;

    private void Awake()
    {
        SetCameraDimensions();
    }

    private void Update()
    {
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        float minCamX = minX + camHalfWidth;
        float maxCamX = maxX - camHalfWidth;
        float minCamY = minY + camHalfHeight;
        float maxCamY = maxY - camHalfHeight;

        float clampedX = Mathf.Clamp(desiredPosition.x, minCamX, maxCamX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minCamY, maxCamY);

        transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);
    }

    public void SetCameraDimensions()
    {
        camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = camHalfHeight * Camera.main.aspect;
    }
}