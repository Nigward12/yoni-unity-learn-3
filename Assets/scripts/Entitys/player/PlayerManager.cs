using UnityEngine;
using System.IO;
public class PlayerSessionData
{
    public bool inSession = false;
    public float currentHealth;
}
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance { get; private set; }

    public PlayerData playerData;

    [SerializeField] private GameObject playerPrefab;

    private GameObject currentPlayer;
    private PlayerSessionData currentPlayerSessionData = new PlayerSessionData();

    public bool IsPlayerSpawned { get; private set; } = false;
    private string playerDataSaveFilePath => Application.persistentDataPath + "/playerdata.json";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (GameManager.instance.testing)
        {
            IsPlayerSpawned = true;
            currentPlayer = GameObject.Find("Test Player");
        }
    }

    public void SavePlayerSessionData()
    {
        Health playerHealth = currentPlayer.GetComponent<Health>();
        currentPlayerSessionData.currentHealth = playerHealth.currentHealth;
        // more stuff later ig.......
    }

    public GameObject SpawnPlayer(Vector3 spawnPosition)
    {
        currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        Health newPlayerHealth = currentPlayer.GetComponent<Health>();

        newPlayerHealth.fullHealth = playerData.PlayerMaxHealth;
        if (currentPlayerSessionData.inSession)
            newPlayerHealth.SetHealth(currentPlayerSessionData.currentHealth);
        else
        {
            currentPlayerSessionData.inSession = true;
            newPlayerHealth.SetHealth(newPlayerHealth.fullHealth);
        }
        // more stuff later ig.......
        IsPlayerSpawned = true;
        return currentPlayer;
    }

    public void DisablePlayerMovement()
    {
        currentPlayer.GetComponent<PlayerBasicMovement>().enabled = false;
    }

    public void EnablePlayerMovement()
    {
        currentPlayer.GetComponent<PlayerBasicMovement>().enabled = true;
    }

    public void SetPlayerSpawned(bool status)
    {
        IsPlayerSpawned = status;
    }

    public void LoadPlayerData()
    {
        if (File.Exists(playerDataSaveFilePath))
        {
            string json = File.ReadAllText(playerDataSaveFilePath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }

    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(playerDataSaveFilePath, json);
    }

    public GameObject getCurrentPlayer()
    {
        return currentPlayer;
    }
}
