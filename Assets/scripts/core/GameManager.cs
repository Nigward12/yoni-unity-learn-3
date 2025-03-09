using UnityEngine;
using static System.TimeZoneInfo;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private LevelData mainMenuLevelData;
    [SerializeField] private LevelData firstLevelData;
    [SerializeField] private float minTransitionToGameTime = 1f;

    [SerializeField] private PlayerData playerData;

    [Header("DEBUG")]
    [SerializeField] private bool loadWithoutSaves;
 
    private bool inMainMenu;

    private const string SAVED_LEVEL_KEY = "SavedGameCurrentLevel";
    private const string SAVED_CHECKPOINT_KEY = "SavedGameCurrentCheckpoint";

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

        InMainMenu();
    }

    public void InMainMenu()
    {
        inMainMenu = true;
        SoundManager.instance.ChangeNPlayMusic(mainMenuLevelData.levelMusic);
    }

    public void PlayGame()
    {
        if (inMainMenu)
        {
            PlayerManager.instance.LoadPlayerData();

            if (PlayerPrefs.HasKey(SAVED_LEVEL_KEY) && !loadWithoutSaves)
                LoadSavedGame();
            else
                LoadGameWithoutSaves();

            inMainMenu = false;

        }
    }


    #region load game
    private void LoadSavedGame()
    {
        string savedLevelName = PlayerPrefs.GetString(SAVED_LEVEL_KEY);
        LevelData savedLevelData = Resources.Load<LevelData>($"LevelData/{savedLevelName}Data");

        if (PlayerPrefs.HasKey(SAVED_CHECKPOINT_KEY))
        {
            string savedCheckpointName = PlayerPrefs.GetString(SAVED_CHECKPOINT_KEY);
            PlayerRespawnManager.instance.SetCheckpointOnGameStart(savedCheckpointName, savedLevelData);
            LoadingManager.instance.TransitionToScene(savedLevelData, minTransitionToGameTime, true,
                savedCheckpointName);
        }
        else
            LoadingManager.instance.TransitionToScene(savedLevelData, minTransitionToGameTime, true);
    }

    private void LoadGameWithoutSaves()
    {
        LoadingManager.instance.TransitionToScene(firstLevelData, minTransitionToGameTime, true);
    }
    #endregion

    #region save game
    public void SaveInCheckpoint(Checkpoint spawnCheckpoint)
    {

        PlayerManager.instance.SavePlayerData();

        PlayerPrefs.SetString(SAVED_LEVEL_KEY, LoadingManager.instance.currentLevelData.levelName);

        PlayerPrefs.SetString(SAVED_CHECKPOINT_KEY, spawnCheckpoint.name);

        PlayerPrefs.Save();
    }
    #endregion

}
