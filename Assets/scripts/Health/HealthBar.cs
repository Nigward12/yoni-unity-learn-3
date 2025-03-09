using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public static HealthBar instance { get; private set; }
    private Health playerHealth;
    [SerializeField] private Image HealthBarFull;
    [SerializeField] private Image HealthBarEmpty;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    private IEnumerator Start()
    {
        if (!PlayerManager.instance.IsPlayerSpawned)
        {
            this.enabled = false;
            while (!PlayerManager.instance.IsPlayerSpawned)
            {
                yield return null; 
            }
            this.enabled = true; 
        }

        playerHealth = PlayerManager.instance.getCurrentPlayer().GetComponent<Health>();
        HealthBarEmpty.fillAmount = 0;
    }

    private void Update()
    {
        if (PlayerManager.instance.IsPlayerSpawned)
        {
            HealthBarFull.fillAmount = playerHealth.currentHealth / playerHealth.fullHealth;
            HealthBarEmpty.fillAmount = 1 - HealthBarFull.fillAmount;
        }
        else
            StartCoroutine(Start());
    }
}
