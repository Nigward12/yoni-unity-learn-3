using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    private Checkpoint currentCheckpoint;
    private Health playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
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
        UiManager.instance.DeathUi();
        CinemachineCameraManager.instance.SwapToCheckpointCamera(currentCheckpoint.camInCheckpoint);
        currentCheckpoint.camInCheckpoint.Target.TrackingTarget = currentCheckpoint.transform;

        while (UiManager.instance.IsDeathScreenActive())
        {
            yield return null; 
        }

        transform.position = currentCheckpoint.transform.position + Vector3.up * 3f;
        playerHealth.OnRespawn();
        currentCheckpoint.OnRespawnInCheckpoint();
    }
}
