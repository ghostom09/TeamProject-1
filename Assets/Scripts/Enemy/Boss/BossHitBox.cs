using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    private float damage;
    private LayerMask targetLayer;

    public void Init(float damage, GameObject target)
    {
        this.damage = damage;
        targetLayer = target.layer;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != targetLayer) return;
        
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            Vector2 dir = other.transform.position - transform.parent.position;
            target.TakeDamage(damage);
            target.ApplyKnockback(dir, 3f, 0.15f);
        }
    }
}
