using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }
    public LevelData currentLevelData;
    public bool isLoading = false;

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

    public void TransitionToScene(LevelData levelData, float transitionTime, bool firstSceneLoad
        , string spawnCheckpoint = "")
    {
        isLoading = true;
        StartCoroutine(TransitionToSceneCoroutine(levelData, transitionTime, firstSceneLoad, spawnCheckpoint));
    }
    private IEnumerator TransitionToSceneCoroutine(LevelData levelData, float minTransitionTime, bool firstSceneLoad
        , string spawnCheckpoint)
    {
        PreTransition(firstSceneLoad);
        yield return StartCoroutine(UiManager.instance.TransitionFadeIn(minTransitionTime / 2));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelData.levelName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;

        yield return new WaitForSecondsRealtime(0.1f);

        SpawnPlayerInNewScene(levelData, spawnCheckpoint);

        ApplyLevelData(levelData,firstSceneLoad, spawnCheckpoint);

        currentLevelData = levelData;

        isLoading = false;

        yield return StartCoroutine(UiManager.instance.TransitionFadeOut(minTransitionTime / 2));
    }

    private void PreTransition(bool firstSceneLoad)
    {
        PlayerManager.instance.SetPlayerSpawned(false);

        if (!firstSceneLoad)
        {
            PlayerManager.instance.DisablePlayerMovement();
            PlayerManager.instance.SavePlayerSessionData();
        }
    }

    private void SpawnPlayerInNewScene(LevelData levelData, string spawnCheckpoint)
    {
        GameObject spawnPoint;
        Vector3 spawnPosition;

        if (spawnCheckpoint != "")
            spawnPoint = GameObject.Find(spawnCheckpoint);
        else
            spawnPoint = GameObject.Find(levelData.TransitionInfoByFromScene[currentLevelData.levelName].spawnPoint);

        spawnPosition = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
        PlayerManager.instance.SpawnPlayer(spawnPosition);
    }

    private void ApplyLevelData(LevelData levelData, bool firstSceneLoad, string spawnCheckpoint)
    {
        SetCameraOnLevelLoad(levelData, firstSceneLoad, spawnCheckpoint);

        SoundManager.instance.ChangeMusic(levelData.levelMusic);

        if (firstSceneLoad)
            UiManager.instance.OnGamePlay();
    }

    private void SetCameraOnLevelLoad(LevelData levelData, bool firstSceneLoad, string spawnCheckpoint)
    {
        GameObject targetAfterTransition;
        CinemachineCamera camAfterTransition;

        if (spawnCheckpoint != "")
        {
            Checkpoint checkpoint = GameObject.Find(spawnCheckpoint).GetComponent<Checkpoint>();
            targetAfterTransition = checkpoint.checkpointCamTarget.gameObject;
            camAfterTransition = checkpoint.camInCheckpoint;
        }
        else
        {
            TransitionInfo transitionInfo = levelData.TransitionInfoByFromScene[currentLevelData.levelName];
            targetAfterTransition = GameObject.Find(transitionInfo.camTargetName);
            camAfterTransition = CinemachineCameraManager.instance.
                GetCameraInstanceByPrefab(transitionInfo.camActiveAfterTransitionPrefab);
        }

        CinemachineCameraManager.instance.CutCamToTarget(camAfterTransition,
            targetAfterTransition.transform);
        CinemachineCameraManager.instance.SwapCameraGeneric(camAfterTransition);
    }


    // add spawning in checkpoint
}
