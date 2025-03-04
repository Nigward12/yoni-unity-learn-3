using UnityEngine;
using System.Collections;

public class PlayerCameraTarget : MonoBehaviour
{
    [Header("Flip Rotation stats")]
    [SerializeField]
    private float _flipYRotationTime = 0.5f;
    private bool track = true;

    public static PlayerCameraTarget instance { get; private set; }

    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
    }
    void Update()
    {
        if (track)
            transform.position = PlayerManager.instance.transform.position;
    }

    public void StopTracking()
    {
        track = false;
    }

    public void StartTracking()
    {
        track = true;
    }

    public void CallTurn(float endRotation)
    {
        LeanTween.rotateY(gameObject, endRotation, _flipYRotationTime).setEaseInOutSine();
    }

}
