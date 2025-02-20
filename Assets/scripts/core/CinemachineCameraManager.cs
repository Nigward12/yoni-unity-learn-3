using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CinemachineCameraManager : MonoBehaviour
{
    // add the lerping itself from this script or the movement script
    public static CinemachineCameraManager instance;

    [SerializeField] private CinemachineCamera[] Cameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    [SerializeField] private float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer positionComposer;

    private float normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < Cameras.Length; i++)
        {
            if (Cameras[i].enabled)
            {
                _currentCamera = Cameras[i];
                positionComposer = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body)
                    as CinemachinePositionComposer;
            }
        }

        normYPanAmount = positionComposer.Damping.y;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        // Grab the starting Y Damping value
        float startDampAmount = positionComposer.Damping.y;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
            endDampAmount = normYPanAmount;

        // Lerp the damping amount over time
        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallYPanTime);
            positionComposer.Damping.y = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion
}
