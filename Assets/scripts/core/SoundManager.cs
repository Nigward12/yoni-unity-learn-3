
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip audioClip;
    public float pitch = 1f;
    public float volume = 1f;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    [SerializeField] private Sound music;
    private AudioSource MusicSource;

    private void Awake()
    {
        MusicSource = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
            Destroy(gameObject);

        PlayMusicLoop();
    }

    public void ChangeMusic(Sound newMusic)
    {
        StopMusicLoop();

        music = newMusic;

        PlayMusicLoop();
    }

    public void PlayMusicLoop()
    {
        MusicSource.clip = music.audioClip;
        MusicSource.pitch = music.pitch;
        MusicSource.volume = music.volume;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void StopMusicLoop()
    {
        if (MusicSource.isPlaying)
        {
            MusicSource.Stop(); 
        }
    }

    public AudioSource PlaySound(Sound _sound)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = _sound.audioClip;
        tempSource.pitch = _sound.pitch;
        tempSource.volume = _sound.volume;
        tempSource.Play();

        Destroy(tempSource, _sound.audioClip.length / _sound.pitch);

        return tempSource;
    }

    public AudioSource PlayLoopingSound(Sound _sound)
    {
        GameObject soundObject = new GameObject("LoopingSound");
        soundObject.transform.SetParent(transform);

        AudioSource tempSource = soundObject.AddComponent<AudioSource>();
        tempSource.clip = _sound.audioClip;
        tempSource.pitch = _sound.pitch;
        tempSource.volume = _sound.volume;
        tempSource.loop = true;
        tempSource.Play();

        return tempSource;
    }
}
