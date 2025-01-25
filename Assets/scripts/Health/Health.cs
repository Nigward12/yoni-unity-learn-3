using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float startingHealth;
    [SerializeField] private List<MonoBehaviour> disableOnDeath;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] Color flashColor;
    [SerializeField] int enemyLayerCode;
    [SerializeField] int playerLayerCode;
    private SpriteRenderer spriteRend;


    private void Awake()
    {
        dead = false;
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            if (iFramesDuration > 0)
                StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("death");
                disableScriptsOnDeath();
                dead = true;
            }
        }
    }
    public void disableScriptsOnDeath()
    {
        foreach(MonoBehaviour script in disableOnDeath)
            script.enabled = false;
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(playerLayerCode, enemyLayerCode, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = flashColor;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(playerLayerCode, enemyLayerCode, false);
    }
}
