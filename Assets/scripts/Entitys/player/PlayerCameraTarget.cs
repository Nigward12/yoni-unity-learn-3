using UnityEngine;
using System.Collections;

public class PlayerCameraTarget : MonoBehaviour
{
    [Header("Flip Rotation stats")]
    [SerializeField]
    private float _flipYRotationTime = 0.5f;

    public static PlayerCameraTarget instance { get; private set; }

    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
    }
    void Update()
    {
        transform.position = PlayerManager.instance.transform.position;
    }

    public void CallTurn(float endRotation)
    {
        LeanTween.rotateY(gameObject, endRotation, _flipYRotationTime).setEaseInOutSine();
    }

}
