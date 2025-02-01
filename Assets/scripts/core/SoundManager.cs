using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public AudioClip audioClip;
    public float pitch = 1f;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound _sound)
    {
        //float originalPitch = source.pitch;
        //source.pitch = _sound.pitch;

        //source.PlayOneShot(_sound.audio);

        //source.pitch = originalPitch;

        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = _sound.audioClip;
        tempSource.pitch = _sound.pitch;
        tempSource.Play();

        Destroy(tempSource, _sound.audioClip.length / _sound.pitch);

    }
}
