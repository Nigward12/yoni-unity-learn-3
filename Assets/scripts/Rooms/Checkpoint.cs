using NUnit.Framework.Constraints;
using Unity.Cinemachine;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sound checkpointSound;

    [Header("checkpoint cam settings")]
    public CinemachineCamera camInCheckpoint;
    public Transform checkpointCamTarget;
    //[SerializeField] private CamBorderSetter cpCamBorderSetter;
    [SerializeField] private SoundsSetter checkpointSoundSetter;
    
    private bool checkpointDiscovered;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerRespawnManager.instance.SetCheckPoint(this);
            if (!checkpointDiscovered)
            {
                checkpointDiscovered = true;
                SoundManager.instance.PlaySound(checkpointSound);
            }
            GameManager.instance.SaveInCheckpoint(this);
        }
    }

    //public void OnRespawnInCheckpoint()
    //{
    //    if (cpCamBorderSetter.gameObject.activeSelf)
    //    {
    //        cpCamBorderSetter.BorderSet();
    //    }

    //    camInCheckpoint.Target.TrackingTarget = checkpointCamTarget;

    //    checkpointSoundSetter.SoundSet();
    //    play checkpoint animations or somethin...
    //}
}
