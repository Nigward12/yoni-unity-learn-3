using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SoundsSetter : MonoBehaviour
{

    public List<Sound> atmosphereSounds;
    public Sound music;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            SoundSet();
    }

    public void SoundSet()
    {
        if (atmosphereSounds.Count > 0 &&
                !atmosphereSounds.Equals(SoundManager.instance.GetAtmosphereSounds()))
            SoundManager.instance.AddAtmosphereSounds(atmosphereSounds);
        if (music.audioClip != null)
            SoundManager.instance.ChangeMusic(music);
    }
}
