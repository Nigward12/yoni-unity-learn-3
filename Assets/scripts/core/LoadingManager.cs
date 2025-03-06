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
    private IEnumerator TransitionToSceneCoroutine(LevelData levelData, float minTransitionTime, GameObject camActiveAfterTransitionPrefab)
    {
        PlayerManager.instance.SavePlayerSessionData();

        yield return StartCoroutine(UiManager.instance.TransitionFadeIn(minTransitionTime / 2));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelData.levelName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByName(levelData.levelName);
        SpawnPlayerInNewScene(newScene,levelData);

        asyncLoad.allowSceneActivation = true;

        yield return new WaitForSecondsRealtime(0.1f);

        SceneManager.SetActiveScene(newScene);

        ApplyLevelData(levelData, camActiveAfterTransitionPrefab);

        yield return StartCoroutine(UiManager.instance.TransitionFadeOut(minTransitionTime / 2));
    }

    private void SpawnPlayerInNewScene(Scene newScene, LevelData levelData)
    {
        GameObject spawnPoint = GameObject.Find(levelData.TransitionInfoByFromScene[currentLevelData.levelName].spawnPoint);
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;

        GameObject newPlayer = PlayerManager.instance.CreateNewPlayer(spawnPosition);

        SceneManager.MoveGameObjectToScene(newPlayer, newScene);
    }

    private void ApplyLevelData(LevelData levelData, GameObject camActiveAfterTransitionPrefab)
    {

        GameObject targetAfterTransition = GameObject.Find(levelData.TransitionInfoByFromScene
            [currentLevelData.levelName].camTargetName);

        CinemachineCamera camAfterTransition = CinemachineCameraManager.instance.
            GetCameraInstanceByPrefab(camActiveAfterTransitionPrefab);

        CinemachineCameraManager.instance.CutCamToTarget(camAfterTransition,
            targetAfterTransition.transform);

        CinemachineCameraManager.instance.SwapCameraGeneric(camAfterTransition);
    }
}
