using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{
    public static UiManager instance {  get; private set; }
    [Header ("Main Menu")]
    [SerializeField] private GameObject mainMenuScreen;

    [Header ("death")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Sound deathSound;

    [Header("pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header ("level transition")]
    [SerializeField] private Image levelTransitionScreen;
    [SerializeField] private Color LTSColor = Color.black;

    [Header ("UI indicators")]
    [SerializeField] private GameObject healthBar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            EventSystem existingEventSystem = FindFirstObjectByType<EventSystem>();
            if (existingEventSystem != null && existingEventSystem != GetComponent<EventSystem>())
            {
                Destroy(existingEventSystem.gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
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
            PauseGame(!pauseScreen.activeInHierarchy);
    }

    public void OnGamePlay()
    {
        mainMenuScreen.SetActive(false);
        healthBar.SetActive(true);
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
            pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            SoundManager.instance.ResumeAllSounds();
            pauseScreen.SetActive(false);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        // set isMainMenu to true in game manager , also play music
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

    public void EnterTransitionState(bool status)
    {
        if (status)
        {
            this.enabled = false;
            Time.timeScale = 0;
            SoundManager.instance.DestroyAllSounds();
            SoundManager.instance.StopMusicLoop();
        }
        else
        {
            Time.timeScale = 1;
            this.enabled = true;
            SoundManager.instance.PlayMusicLoop();
        }
    }
    public IEnumerator TransitionFadeIn(float fadeDuration)
    {
        EnterTransitionState(true);
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            levelTransitionScreen.color = new Color(LTSColor.r, LTSColor.g, LTSColor.b,
                Mathf.Lerp(0, 1, t / fadeDuration));
            yield return null;
        }
    }

    public IEnumerator TransitionFadeOut(float fadeDuration)
    {
        EnterTransitionState(false);
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            levelTransitionScreen.color = new Color(LTSColor.r, LTSColor.g, LTSColor.b,
                Mathf.Lerp(1, 0, t / fadeDuration));
            yield return null;
        }
    }
    #endregion
}
