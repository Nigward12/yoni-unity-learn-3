using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.IsTouching(GetComponent<Collider2D>()))
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private void OnDisable()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
