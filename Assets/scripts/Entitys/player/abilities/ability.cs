using UnityEngine;

public class ability : MonoBehaviour
{
    public KeyCode abilityActivationKey;
    public float cooldownTime;
    private float cooldownTimer;

    public virtual void ActivateAbility()
    {

    }
}
