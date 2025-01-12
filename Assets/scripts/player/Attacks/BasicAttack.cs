using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] private LayerMask enemyLayers;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        TryDamageEnemy();
    }


    public void TryDamageEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, enemyLayers);
        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
