using NUnit.Framework.Constraints;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sound checkpointSound;

    [Header("checkpoint cam settings")]
    public CinemachineCamera camInCheckpoint;
    public Transform checkpointCamTarget;
    [SerializeField] private SoundsSetter checkpointSoundSetter;
    
    private bool checkpointDiscovered;
    private string checkpointKey;

    private void Awake()
    {
        checkpointKey = this.name +"_"+SceneManager.GetActiveScene().name;
        if (!GameManager.instance.loadWithoutSaves)
            checkpointDiscovered = CheckpointManager.instance.IsCheckpointDiscovered(checkpointKey);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerRespawnManager.instance.SetCheckPoint(this);
            if (!checkpointDiscovered)
            {
                checkpointDiscovered = true;
                SoundManager.instance.PlaySound(checkpointSound);
                CheckpointManager.instance.DiscoverCheckpoint(checkpointKey);
            }
            SaveManager.SaveInCheckpoint(this);
        }
    }

    public void OnRespawnInCheckpoint()
    {
    //    if (cpCamBorderSetter.gameObject.activeSelf)
    //    {
    //        cpCamBorderSetter.BorderSet();
    //    }

    //    camInCheckpoint.Target.TrackingTarget = checkpointCamTarget;

       checkpointSoundSetter.SoundSet();
    //    play checkpoint animations or somethin...
    }
}
