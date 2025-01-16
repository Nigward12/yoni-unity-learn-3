using UnityEngine;

public class CamBorderSetter : MonoBehaviour
{
    [SerializeField] private CameraController mainCamera;
    [SerializeField] private Transform minXBorder;
    [SerializeField] private Transform maxXBorder;
    [SerializeField] private Transform minYBorder;
    [SerializeField] private Transform maxYBorder;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mainCamera.minX = minXBorder.position.x;
        mainCamera.maxX = maxXBorder.position.x;
        mainCamera.minY = minYBorder.position.y;
        mainCamera.maxY = maxYBorder.position.y;
    }

}
