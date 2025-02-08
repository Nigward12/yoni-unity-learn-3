using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image HealthBarFull;
    [SerializeField] private Image HealthBarEmpty;

    private void Awake()
    {
        HealthBarEmpty.fillAmount = 0;
    }

    private void Update()
    {
        HealthBarFull.fillAmount = playerHealth.currentHealth / playerHealth.fullHealth;
        HealthBarEmpty.fillAmount = 1 - HealthBarFull.fillAmount;
    }
}
