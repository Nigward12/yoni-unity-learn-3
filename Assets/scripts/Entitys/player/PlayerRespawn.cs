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
        StartCoroutine(RespawnAfterDeathScreen());
    }

    private IEnumerator RespawnAfterDeathScreen()
    {
        uiManager.DeathUi();

        while (uiManager.IsDeathScreenActive())
        {
            yield return null; 
        }

        transform.position = currentCheckpoint.transform.position + Vector3.up * 3f;
        playerHealth.OnRespawn();
        currentCheckpoint.OnRespawnInCheckpoint();
    }
}
