using UnityEngine;

public class HitBox : MonoBehaviour
{
    private float damage;

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            Vector2 dir = other.transform.position - transform.position;
            target.TakeDamage(damage);
            target.ApplyKnockback(dir, 3f, 0.15f);
        }
    }
}
