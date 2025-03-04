

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

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
    private List<AudioSource> activeSoundSources = new List<AudioSource>();
    private float soundRatio = 1, musicRatio = 1;
    private bool canChangeSoundList = true;

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

        ChangeSoundVolumeRatio(0);
        ChangeMusicVolumeRatio(0);

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
        MusicSource.volume = music.volume * musicRatio;
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
        if (canChangeSoundList)
        {
            AudioSource tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.clip = _sound.audioClip;
            tempSource.pitch = _sound.pitch;
            tempSource.volume = _sound.volume * soundRatio;
            tempSource.Play();

            activeSoundSources.Add(tempSource);
            StartCoroutine(RemoveSoundAfterPlaying(tempSource, _sound.audioClip.length / _sound.pitch));

            return tempSource;
        }
        return null;
    }

    public AudioSource PlayLoopingSound(Sound _sound)
    {
        if (canChangeSoundList)
        {
            GameObject soundObject = new GameObject("LoopingSound");
            soundObject.transform.SetParent(transform);

            AudioSource tempSource = soundObject.AddComponent<AudioSource>();
            tempSource.clip = _sound.audioClip;
            tempSource.pitch = _sound.pitch;
            tempSource.volume = _sound.volume * soundRatio;
            tempSource.loop = true;
            tempSource.Play();
            activeSoundSources.Add(tempSource);

            return tempSource;
        }
        return null;
    }

    public void StopLoopingSound(AudioSource loopSource)
    {
        if (canChangeSoundList && activeSoundSources.Contains(loopSource))
        {
            activeSoundSources.Remove(loopSource);
            Destroy(loopSource);
        }
    }

    private IEnumerator RemoveSoundAfterPlaying(AudioSource source, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        if (activeSoundSources.Contains(source))
        {
            activeSoundSources.Remove(source);
            Destroy(source);
        }
    }

    public void DestroyAllSounds()
    {
        canChangeSoundList = false;
        foreach (AudioSource source in activeSoundSources)
        {
            if (source.isPlaying)
            {
                activeSoundSources.Remove(source);
                Destroy(source);
            }
        }
        canChangeSoundList = true;
    }
    public void PauseAllSounds()
    {
        foreach (AudioSource source in activeSoundSources)
        {
            if (source.isPlaying)
                source.Pause();
        }
    }

    public void ResumeAllSounds()
    {
        foreach (AudioSource source in activeSoundSources)
        {
            if (!source.isPlaying)
            {
                source.volume *= soundRatio;
                source.Play();
            }
        }
    }

    public void ChangeSoundVolumeRatio(float change)
    {
        ChangeVolumeRatio("soundVolume", ref soundRatio, change);
    }

    public void ChangeMusicVolumeRatio(float change)
    {
        ChangeVolumeRatio("musicVolume", ref musicRatio, change);
        MusicSource.volume = music.volume * musicRatio;
    }

    private void ChangeVolumeRatio(string volumeNameInPrefs,ref float ratioObject, float change)
    {
        float currentVolumeRatio = PlayerPrefs.GetFloat(volumeNameInPrefs, 1);

        currentVolumeRatio += change;

        if (currentVolumeRatio > 1)
            currentVolumeRatio = 0;
        else if (currentVolumeRatio < 0)
            currentVolumeRatio = 1;

        ratioObject = currentVolumeRatio;

        PlayerPrefs.SetFloat(volumeNameInPrefs, currentVolumeRatio);
    }
}
