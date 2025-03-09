using UnityEngine;
using System.Collections;

public class PlayerCameraTarget : MonoBehaviour
{
    [Header("Flip Rotation stats")]
    [SerializeField]
    private float _flipYRotationTime = 0.5f;
    private Transform player;

    public static PlayerCameraTarget instance { get; private set; }

    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
    }
    private IEnumerator Start()
    {
        if (!PlayerManager.instance.IsPlayerSpawned)
        {
            this.enabled = false;
            while (!PlayerManager.instance.IsPlayerSpawned)
            {
                yield return null;
            }
            this.enabled = true;
        }
        player = PlayerManager.instance.getCurrentPlayer().transform;
    }
    private void Update()
    {
        transform.position = player.transform.position;
    }

    public void CallTurn(float endRotation)
    {
        LeanTween.rotateY(gameObject, endRotation, _flipYRotationTime).setEaseInOutSine();
    }

}
