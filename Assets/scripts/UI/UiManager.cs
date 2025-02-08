using UnityEngine;
using System.Collections;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Sound deathSound;

    public void DeathUi()
    {
        deathScreen.SetActive(true);
        StartCoroutine(HandleDeathScreen());
    }
    private IEnumerator HandleDeathScreen()
    {
        AudioSource soundSource = SoundManager.instance.PlaySound(deathSound);

        while (soundSource != null && soundSource.isPlaying)
        {
            yield return null; 
        }

        deathScreen.SetActive(false);
    }

    public bool IsDeathScreenActive()
    {
        return deathScreen.activeSelf;
    }
}
