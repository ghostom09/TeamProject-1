using UnityEngine;

public class MeleeAttack : IEnemyAttackStrategy
{
    private float damage;
    private float attackSpeed;
    private float lastAttackTime;

    private float distSqr;
    private float interval;

    private const float FIXED_RANGE = 1.5f;

    public void Init(EnemyStats stats, EnemyMove move)
    {
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
    }

    public bool TryAttack(GameObject self, Transform target, Vector2 direction)
    {
        distSqr = (target.position - self.transform.position).sqrMagnitude;

        if (distSqr > FIXED_RANGE * FIXED_RANGE)
            return false;

        interval = 1f / attackSpeed;

        if (Time.time < lastAttackTime + interval)
            return false;

        lastAttackTime = Time.time;

        target.GetComponent<IDamageable>()?.TakeDamage(damage);
        return true;
    }
}
