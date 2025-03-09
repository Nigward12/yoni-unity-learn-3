using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public void Respawn()
    {
        PlayerRespawnManager.instance.Respawn();
    }
}
