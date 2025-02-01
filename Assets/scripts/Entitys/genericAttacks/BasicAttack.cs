using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] private LayerMask enemyLayers;
    //public Sound attackSound;
    private Collider2D Collider2D;
    private ContactFilter2D contactFilter;

    [Header("SFX")]
    [SerializeField] private Sound hitSound;

    private void Awake()
    {
        Collider2D = GetComponent<Collider2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayers);
        contactFilter.useTriggers = true;
    }

    private void OnEnable()
    {
        Collider2D.enabled = true;

        TryDamageEnemy();

        Collider2D.enabled = false;
    }

    //public void PlayAttackSound()
    //{
    //    SoundManager.instance.PlaySound(attackSound);
    //}


    public void TryDamageEnemy()
    {
        List<Collider2D> hits = new List<Collider2D>();

        Collider2D.Overlap(contactFilter, hits);

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<Health>().TakeDamage(damage);
            if (hitSound.audioClip)
                SoundManager.instance.PlaySound(hitSound);
        }
    }
}
