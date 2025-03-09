using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionPoint : MonoBehaviour
{
    [SerializeField] private LevelData nextLevelData;
    [SerializeField] private float minTransitionTime;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CinemachineCameraManager.instance.TargetSwapGeneric(transform);
            LoadingManager.instance.TransitionToScene(nextLevelData, minTransitionTime, false);
        }
    }

}
