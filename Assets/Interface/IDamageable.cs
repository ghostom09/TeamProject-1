using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage);
    public void ApplySlow(float percent, float duration);
    
    void ApplyKnockback(Vector2 dir, float power, float duration);
}