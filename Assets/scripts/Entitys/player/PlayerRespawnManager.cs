using UnityEngine;
using System.Collections;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager instance {  get; private set; }
    private string currentCheckpointName;
    private LevelData currentCheckpointLD;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetCheckpointOnGameStart(string startCheckpointName, LevelData startCheckpointLD)
    {
        currentCheckpointName = startCheckpointName;
        currentCheckpointLD = startCheckpointLD;
    }
    public void SetCheckPoint(Checkpoint checkpoint)
    {
        currentCheckpointName = checkpoint.name;
        currentCheckpointLD = LoadingManager.instance.currentLevelData;
        //maybe add the leveldata as a property of the checkpoint script
    }

    public void Respawn()
    {
        SoundManager.instance.StopMusicLoop();
        SoundManager.instance.DestroyAllSounds();
        StartCoroutine(RespawnAfterDeathScreen());
    }

    private IEnumerator RespawnAfterDeathScreen()
    {
        UiManager.instance.DeathUi();

        while (UiManager.instance.IsDeathScreenActive())
            yield return null;

        LoadingManager.instance.TransitionToScene(currentCheckpointLD, 1f, false,
                currentCheckpointName);

        while (LoadingManager.instance.isLoading)
            yield return null;

        PlayerManager.instance.getCurrentPlayer().GetComponent<Health>().OnRespawn();
    }
}
