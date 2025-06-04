using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;

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

    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    MapRoomManager.instance.RevealRoom();
    //}

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
        Checkpoint checkpoint = null;

        if (spawnCheckpoint != "")
        {
            checkpoint = GameObject.Find(spawnCheckpoint).GetComponent<Checkpoint>();
            checkpoint.OnRespawnInCheckpoint();
        }

        SetSoundsOnLevelLoad(levelData);
        SetCameraOnLevelLoad(levelData, firstSceneLoad, checkpoint);

        if (firstSceneLoad)
            UiManager.instance.OnGamePlay();
    }

    private void SetSoundsOnLevelLoad(LevelData levelData)
    {
        SoundManager.instance.ChangeMusic(levelData.levelMusic);
    }

    private void SetCameraOnLevelLoad(LevelData levelData, bool firstSceneLoad, Checkpoint spawnCheckpoint)
    {
        GameObject targetAfterTransition;
        CinemachineCamera camAfterTransition;

        if (spawnCheckpoint != null)
        {
            targetAfterTransition = spawnCheckpoint.checkpointCamTarget.gameObject;
            camAfterTransition = spawnCheckpoint.camInCheckpoint;
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

}
