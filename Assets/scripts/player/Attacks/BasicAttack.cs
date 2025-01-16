using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] private LayerMask enemyLayers;
    private PolygonCollider2D polCollider;
    private ContactFilter2D contactFilter;

    private void Awake()
    {
        polCollider = GetComponent<PolygonCollider2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayers);
        contactFilter.useTriggers = true;
    }

    private void OnEnable()
    {
        TryDamageEnemy();
    }


    public void TryDamageEnemy()
    {
        List<Collider2D> hits = new List<Collider2D>();

        polCollider.Overlap(contactFilter, hits);

        foreach (Collider2D hit in hits)
        {
            print(hit.gameObject.name + "  " + gameObject.name);

            hit.GetComponent<Health>().TakeDamage(damage); ;
        }
    }
}
