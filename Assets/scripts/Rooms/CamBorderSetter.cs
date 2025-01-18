using UnityEngine;
using System.Collections;

public class CamBorderSetter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform minXBorder;
    [SerializeField] private Transform maxXBorder;
    [SerializeField] private Transform minYBorder;
    [SerializeField] private Transform maxYBorder;

    private float minX, maxX, minY, maxY;
    private CameraController mainCamera;
    private float roomWidth, roomHeight, requiredSizeBasedOnHeight,
        requiredSizeBasedOnWidth, targetSize;

    private void Awake()
    {
        mainCamera = cam.GetComponent<CameraController>();
        minX = minXBorder.position.x;
        maxX = maxXBorder.position.x;
        minY = minYBorder.position.y;
        maxY = maxYBorder.position.y;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AdjustCamSize();
        SwitchToNewBorders();
    }

    private void SwitchToNewBorders()
    {
        mainCamera.minX = minX;
        mainCamera.maxX = maxX;
        mainCamera.minY = minY;
        mainCamera.maxY = maxY;
    }


    private void AdjustCamSize()
    {
        roomWidth = maxX - minX;
        roomHeight = maxY - minY;

        requiredSizeBasedOnHeight = roomHeight / 2f;

        requiredSizeBasedOnWidth = (roomWidth / 2f) / cam.aspect;

        targetSize = Mathf.Min(requiredSizeBasedOnHeight, requiredSizeBasedOnWidth);

        targetSize = Mathf.Min(targetSize, mainCamera.defaultSize);

        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = targetSize;
            //StartCoroutine(SmoothZoom(cam.orthographicSize, targetSize, 0.2f));
            mainCamera.SetCameraDimensions();
        }
    }

    private IEnumerator SmoothZoom(float startSize, float endSize, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);

            yield return null;
        }

        cam.orthographicSize = endSize;

        mainCamera.SetCameraDimensions();
    }

}
