using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sound checkpointSound;
    [SerializeField] private CamBorderSetter cpCamBorderSetter;
    [SerializeField] private Sound musicInCheckpoint;
    
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

        SoundManager.instance.ChangeMusic(musicInCheckpoint);
        // play checkpoint animations or somethin...
    }
}
