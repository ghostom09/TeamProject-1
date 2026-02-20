using UnityEngine;

public class MeleeAttack : IEnemyAttackStrategy
{
    private float damage;
    private float attackSpeed;
    private float lastAttackTime;

    private float distSqr;
    private float interval;

    private const float FIXED_RANGE = 1.5f;

    public void Init(EnemyStats stats)
    {
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
    }

    public void TryAttack(Transform self, Transform target)
    {
        distSqr = (target.position - self.position).sqrMagnitude;

        if (distSqr > FIXED_RANGE * FIXED_RANGE)
            return;

        interval = 1f / attackSpeed;

        if (Time.time < lastAttackTime + interval)
            return;

        lastAttackTime = Time.time;

        target.GetComponent<IDamageable>()?.TakeDamage(damage);
        Debug.Log($"Attacking {target.name}");
    }
}