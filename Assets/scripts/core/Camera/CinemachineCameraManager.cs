using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class CinemachineCameraConfig
{
    public CinemachinePositionComposer PositionComposer { get; }
    public float NormYDampAmount { get; }
    public Vector2 StartingTrackedObjectOffset { get; }

    public CinemachineCameraConfig(
        CinemachinePositionComposer positionComposer,
        float normYDampAmount,
        Vector2 startingTrackedObjectOffset)
    {
        PositionComposer = positionComposer;
        NormYDampAmount = normYDampAmount;
        StartingTrackedObjectOffset = startingTrackedObjectOffset;
    }
}
public class CinemachineCameraManager : MonoBehaviour
{
    public static CinemachineCameraManager instance;

    [SerializeField] private CinemachineBrain brain;

    [SerializeField] private CinemachineCamera[] Cameras;
    private Dictionary<CinemachineCamera, CinemachineCameraConfig> camerasConfigs =
        new Dictionary<CinemachineCamera, CinemachineCameraConfig> ();

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float _fallDampAmount = 0.25f;
    [SerializeField] private float _fallYDampTime = 0.35f;
    [SerializeField] private float _fallSpeedYDampingChangeThreshold = -15f;

    private Rigidbody2D targetPlayerBody;
    private PlayerBasicMovement targetPlayerMovement;
    private bool isLerpingYDamping;
    private bool lerpedFromPlayerFalling;
    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer positionComposer;


    private float normYDampAmount;

    private Vector2 startingTrackedObjectOffset;
    private Dictionary<PanDirection, Vector2> panDirections =
        new Dictionary<PanDirection, Vector2> { { PanDirection.UP, Vector2.up },
            { PanDirection.DOWN,Vector2.down}, { PanDirection.LEFT,Vector2.left},
            {PanDirection.RIGHT ,Vector2.right} };

    private void Awake()
    {

        targetPlayerBody = targetPlayer.GetComponent<Rigidbody2D>();
        targetPlayerMovement = targetPlayer.GetComponent<PlayerBasicMovement>();

        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < Cameras.Length; i++)
        {
            camerasConfigs.Add(Cameras[i], CreateCameraConfig(Cameras[i]));

            if (Cameras[i].enabled)
                _currentCamera = Cameras[i];
        }

        SetCurrentCameraConfig();
    }

    private void Update()
    {
        if (!isLerpingYDamping)
        {
            if (targetPlayerMovement.falling && targetPlayerBody.linearVelocityY
                <= _fallSpeedYDampingChangeThreshold && !lerpedFromPlayerFalling)
                LerpYDamping(true);

            if (!targetPlayerMovement.falling && lerpedFromPlayerFalling)
            {
                lerpedFromPlayerFalling = false;

                LerpYDamping(false);
            }
        }
    }

    private CinemachineCameraConfig CreateCameraConfig(CinemachineCamera camera)
    {
        CinemachinePositionComposer posCom = camera.GetCinemachineComponent
            (CinemachineCore.Stage.Body)  as CinemachinePositionComposer;
        return new CinemachineCameraConfig(posCom, posCom.Damping.y, posCom.TargetOffset);
    }

    private void SetCurrentCameraConfig()
    {
        CinemachineCameraConfig currentCamConfig = camerasConfigs[_currentCamera];
        positionComposer = currentCamConfig.PositionComposer;
        normYDampAmount = currentCamConfig.NormYDampAmount;
        startingTrackedObjectOffset = currentCamConfig.StartingTrackedObjectOffset;
    }
    #region Lerp the Y Damping

    public void LerpYDamping(bool falling)
    {
        if (!brain.IsBlending)
           StartCoroutine(LerpYAction(falling));
    }

    private IEnumerator LerpYAction(bool falling)
    {
        isLerpingYDamping = true;


        float startDampAmount = positionComposer.Damping.y;
        float endDampAmount = 0f;

        if (falling)
        {
            endDampAmount = _fallDampAmount;
            lerpedFromPlayerFalling = true;
        }
        else
            endDampAmount = normYDampAmount;

        float elapsedTime = 0f;
        while (elapsedTime < _fallYDampTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDampAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallYDampTime);
            positionComposer.Damping.y = lerpedDampAmount;

            yield return null;
        }

        isLerpingYDamping = false;
    }

    #endregion

    #region Pan Camera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection,
        bool panToStartingPos)
    {
        StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection,
        bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartingPos)
        {
            endPos = panDirections[panDirection];
            endPos *= panDistance;
            startingPos = startingTrackedObjectOffset;
            endPos += startingPos;
        }
        else
        {
            startingPos = positionComposer.TargetOffset;
            endPos = startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, elapsedTime / panTime);
            positionComposer.TargetOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Camera Swap

    public void SwapCameraHorizontal(CinemachineCamera cameraLeft, CinemachineCamera cameraRight
        ,Vector2 triggerExitDirection)
    {
        if (_currentCamera == cameraLeft && triggerExitDirection.x >0f)
        {
            cameraRight.enabled = true;

            cameraLeft.enabled = false;

            _currentCamera = cameraRight;

            SetCurrentCameraConfig();
        }
        else if (_currentCamera == cameraRight && triggerExitDirection.x <0f)
        {
            cameraLeft.enabled = true;

            cameraRight.enabled = false;

            _currentCamera = cameraLeft;

            SetCurrentCameraConfig();
        }
    }

    public void SwapCameraVertical(CinemachineCamera cameraTop, CinemachineCamera cameraBottom, Vector2 triggerExitDirection)
    {
        if (_currentCamera == cameraTop && triggerExitDirection.y < 0f) 
        {
            cameraBottom.enabled = true;
            cameraTop.enabled = false;
            _currentCamera = cameraBottom;
            SetCurrentCameraConfig();
        }
        else if (_currentCamera == cameraBottom && triggerExitDirection.y > 0f) 
        {
            cameraTop.enabled = true;
            cameraBottom.enabled = false;
            _currentCamera = cameraTop;
            SetCurrentCameraConfig();
        }
    }
    #endregion

    #region Target Swap
    public void TargetSwitchHorizontal(Transform leftTarget, Transform rightTarget, Vector2 triggerExitDirection)
    {
        Transform newTarget = (triggerExitDirection.x > 0) ? rightTarget : leftTarget;

        _currentCamera.Target.TrackingTarget = newTarget;
    }
    #endregion
}
