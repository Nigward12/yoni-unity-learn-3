using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UiManager : MonoBehaviour
{
    public static UiManager instance {  get; private set; }
    [Header ("death")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Sound deathSound;

    [Header("pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header ("level transition")]
    [SerializeField] private Image levelTransitionScreen;
    [SerializeField] private Color LTSColor = Color.black;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        deathScreen.SetActive(false);

        pauseScreen.SetActive(false);

        levelTransitionScreen.color = new Color(LTSColor.r, LTSColor.g, LTSColor.b, 0);
        levelTransitionScreen.gameObject.SetActive(true);
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

    #region level transition
    public IEnumerator TransitionFadeIn(float fadeDuration)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            levelTransitionScreen.color = new Color(LTSColor.r, LTSColor.g, LTSColor.b,
                Mathf.Lerp(0, 1, t / fadeDuration));
            yield return null;
        }
    }

    public IEnumerator TransitionFadeOut(float fadeDuration)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            levelTransitionScreen.color = new Color(LTSColor.r, LTSColor.g, LTSColor.b,
                Mathf.Lerp(1, 0, t / fadeDuration));
            yield return null;
        }
    }
    #endregion
}
