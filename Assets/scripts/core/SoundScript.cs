using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public Sound sound;

    public void OnEnable()
    {
        SoundManager.instance.PlaySound(sound);
    }
}
