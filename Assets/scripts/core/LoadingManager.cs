using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }
    public LevelData currentLevelData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void TransitionToScene(LevelData levelData, float transitionTime,
        GameObject camActiveAfterTransitionPrefab)
    {
        StartCoroutine(TransitionToSceneCoroutine(levelData, transitionTime,
            camActiveAfterTransitionPrefab));
    }
    private IEnumerator TransitionToSceneCoroutine(LevelData levelData, float minTransitionTime,
        GameObject camActiveAfterTransitionPrefab)
    {
        yield return StartCoroutine(UiManager.instance.TransitionFadeIn(minTransitionTime / 2));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelData.levelName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return null;
        ApplyLevelData(levelData, camActiveAfterTransitionPrefab);
        yield return StartCoroutine(UiManager.instance.TransitionFadeOut(minTransitionTime / 2));
    }

    private void ApplyLevelData(LevelData levelData, GameObject camActiveAfterTransitionPrefab)
    {
        PlayerManager.instance.transform.position = GameObject.Find(levelData.TransitionInfoByFromScene
            [currentLevelData.levelName].spawnPoint).transform.position;

        GameObject targetAfterTransition = GameObject.Find(levelData.TransitionInfoByFromScene
            [currentLevelData.levelName].camTargetName);

        CinemachineCamera camAfterTransition = CinemachineCameraManager.instance.
            GetCameraInstanceByPrefab(camActiveAfterTransitionPrefab);

        CinemachineCameraManager.instance.CutCamToTarget(camAfterTransition,
            targetAfterTransition.transform);

        CinemachineCameraManager.instance.SwapCameraGeneric(camAfterTransition);
    }
}
