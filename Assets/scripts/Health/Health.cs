using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float fullHealth;
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

    [Header("SFX")]
    [SerializeField] private Sound hurtSound;


    private void Awake()
    {
        dead = false;
        currentHealth = fullHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, fullHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");

            if (hurtSound.audioClip)
                SoundManager.instance.PlaySound(hurtSound);

            if (iFramesDuration > 0)
                StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                DisEnableScripts(false);
                anim.SetTrigger("death");
                dead = true;
            }
        }
    }
    public void DisEnableScripts(bool enable)
    {
        foreach(MonoBehaviour script in disableOnDeath)
            script.enabled = enable;
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, fullHealth);
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

    public void OnRespawn()
    {
        dead = false;
        AddHealth(fullHealth);
        anim.ResetTrigger("death");
        anim.Play("idle");
        StartCoroutine(Invunerability());
        DisEnableScripts(true);
    }
}
