using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header ("death")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Sound deathSound;

    [Header ("pause")]
    [SerializeField] private GameObject pauseScreen;

    private void Awake()
    {
        deathScreen.SetActive(false);

        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }
    #region death
    public void DeathUi()
    {
        deathScreen.SetActive(true);
        this.enabled = false;
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
        this.enabled = true;
    }

    public bool IsDeathScreenActive()
    {
        return deathScreen.activeSelf;
    }
    #endregion

    #region pause
    public void PauseGame(bool status)
    {
        if (status)
        {
            Time.timeScale = 0;
            SoundManager.instance.PauseAllSounds();
        }
        else
        {
            Time.timeScale = 1;
            SoundManager.instance.ResumeAllSounds();
        }

        pauseScreen.SetActive(status);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolumeRatio(0.05f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolumeRatio(0.05f);
    }

    #endregion
}
