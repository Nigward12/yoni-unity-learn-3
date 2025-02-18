using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sound checkpointSound;
    [SerializeField] private CamBorderSetter cpCamBorderSetter;
    [SerializeField] private SoundsSetter checkpointSoundSetter;
    
    private bool checkpointDiscovered;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerRespawn>().SetCheckPoint(this);
            if (!checkpointDiscovered)
            {
                checkpointDiscovered = true;
                SoundManager.instance.PlaySound(checkpointSound);
            }
        }
    }

    public void OnRespawnInCheckpoint()
    {
        cpCamBorderSetter.BorderSet();

        checkpointSoundSetter.SoundSet();
        // play checkpoint animations or somethin...
    }
}
