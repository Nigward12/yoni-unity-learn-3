using UnityEngine;

public class PlayerSessionData
{
    public float currentHealth;
}
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance { get; private set; }

    [SerializeField] private PlayerData playerData;

    [SerializeField] private GameObject playerPrefab;

    private GameObject currentPlayer;
    private PlayerSessionData currentPlayerSessionData = new PlayerSessionData();

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
    }

    public void SavePlayerSessionData()
    {
        Health playerHealth = currentPlayer.GetComponent<Health>();
        currentPlayerSessionData.currentHealth = playerHealth.currentHealth;
        // more stuff later ig.......
    }

    public GameObject CreateNewPlayer(Vector3 spawnPosition)
    {
        currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        Health newPlayerHealth = currentPlayer.GetComponent<Health>();

        newPlayerHealth.fullHealth = playerData.PlayerMaxHealth;
        newPlayerHealth.SetHealth(currentPlayerSessionData.currentHealth);
        // more stuff later ig.......

        return currentPlayer;
    }

    public GameObject getCurrentPlayer()
    {
        return currentPlayer;
    }
}
