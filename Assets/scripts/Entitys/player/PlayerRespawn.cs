using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    private Checkpoint currentCheckpoint;
    private Health playerHealth;
    private UiManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindFirstObjectByType<UiManager>();
    }

    public void SetCheckPoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void Respawn()
    {
        SoundManager.instance.StopMusicLoop();
        SoundManager.instance.DestroyAllSounds();
        StartCoroutine(RespawnAfterDeathScreen());
    }

    private IEnumerator RespawnAfterDeathScreen()
    {
        uiManager.DeathUi();
        CinemachineCameraManager.instance.SwapToCheckpointCamera(currentCheckpoint.camInCheckpoint);
        currentCheckpoint.camInCheckpoint.Target.TrackingTarget = currentCheckpoint.transform;

        while (uiManager.IsDeathScreenActive())
        {
            yield return null; 
        }

        transform.position = currentCheckpoint.transform.position + Vector3.up * 3f;
        playerHealth.OnRespawn();
        currentCheckpoint.OnRespawnInCheckpoint();
    }
}
