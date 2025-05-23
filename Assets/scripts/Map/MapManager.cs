using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    private Bounds mapBounds;

    public Camera mapCam;
    public GameObject mapUI;
    public float defaultZoom = 35f;
    public float minZoom = 10f;
    public float maxZoom = 100f;
    public float inMapMoveSpeed = 30f;
    public float inMapZoomSpeed = 10f;
    public Transform mainCam;

    private bool isMapOpen = false;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    private void Awake()
    {
        instance = this;
        mapCam.gameObject.SetActive(false);
    }

    private void Start()
    {
        mapBounds = MapRoomManager.instance.masterMapTilemap.localBounds;
        maxZoom = CalculateMaxZoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isMapOpen) OpenMap();
            else CloseMap();
        }

        if (isMapOpen)
        {
            // borders for map movement and saving and loading the map
            UpdateMapMovement();

            UpdateMapDragMovement();

            UpdateMapZoom();

            ClampCameraToBounds();
        }
    }

    public float CalculateMaxZoom()
    {
        Bounds bounds = MapRoomManager.instance.masterMapTilemap.localBounds;

        float sizeX = bounds.size.x / mapCam.aspect;
        float sizeY = bounds.size.y;

        return Mathf.Min(sizeX, sizeY) / 2f;
    }

    private void ClampCameraToBounds()
    {
        float camHeight = mapCam.orthographicSize;
        float camWidth = camHeight * mapCam.aspect;

        Vector3 pos = mapCam.transform.position;

        float minX = mapBounds.min.x + camWidth;
        float maxX = mapBounds.max.x - camWidth;
        float minY = mapBounds.min.y + camHeight;
        float maxY = mapBounds.max.y - camHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        mapCam.transform.position = pos;
    }

    private void UpdateMapMovement()
    {
        if (!isDragging)
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) direction.y += 1;
            if (Input.GetKey(KeyCode.S)) direction.y -= 1;
            if (Input.GetKey(KeyCode.A)) direction.x -= 1;
            if (Input.GetKey(KeyCode.D)) direction.x += 1;

            mapCam.transform.position += direction.normalized * inMapMoveSpeed * Time.unscaledDeltaTime;
        }
    }

    private void UpdateMapDragMovement()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButton(1) && isDragging)
        {
            Vector3 currentMouse = Input.mousePosition;
            Vector3 difference = mapCam.ScreenToWorldPoint(dragOrigin) - mapCam.ScreenToWorldPoint(currentMouse);

            mapCam.transform.position += difference;
            dragOrigin = currentMouse; 
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }

    private void UpdateMapZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        mapCam.orthographicSize -= scroll * inMapZoomSpeed;
        mapCam.orthographicSize = Mathf.Clamp(mapCam.orthographicSize, minZoom, maxZoom);
    }

    public void OpenMap()
    {
        mapCam.gameObject.SetActive(true);
        Transform player = PlayerManager.instance.getCurrentPlayer().transform;
        mapCam.transform.SetParent(null);
        mapCam.transform.position = new Vector3(player.position.x, player.position.y, mapCam.transform.position.z);
        mapCam.orthographicSize = defaultZoom;

        mapUI.SetActive(true);
        isMapOpen = true;

        Time.timeScale = 0;
        PlayerManager.instance.DisablePlayerMovement();
    }

    public void CloseMap()
    {
        mapCam.transform.SetParent(mainCam);
        mapCam.transform.localPosition = Vector3.zero;
        mapCam.orthographicSize = defaultZoom;

        mapUI.SetActive(false);
        mapCam.gameObject.SetActive(false);
        isMapOpen = false;

        Time.timeScale = 1;
        PlayerManager.instance.EnablePlayerMovement();
    }
}