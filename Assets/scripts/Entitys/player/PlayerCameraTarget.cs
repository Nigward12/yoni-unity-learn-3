using UnityEngine;
using System.Collections;

public class PlayerCameraTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform _playerTransform;

    [Header("Flip Rotation stats")]
    [SerializeField]
    private float _flipYRotationTime = 0.5f;

    void Update()
    {
        transform.position = _playerTransform.position;
    }

    public void CallTurn(float endRotation)
    {
        LeanTween.rotateY(gameObject, endRotation, _flipYRotationTime).setEaseInOutSine();
    }

}
