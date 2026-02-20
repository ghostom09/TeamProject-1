using UnityEngine;

public class RangedAttack : IEnemyAttackStrategy
{
    private float damage;
    private float attackSpeed;
    private float attackRange;
    private float lastAttackTime;

    public void Init(EnemyStats stats)
    {
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
        attackRange = stats.attackRange;
    }

    public void TryAttack(Transform self, Transform target)
    {
        float attackInterval = 1f / attackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
        {
            Debug.Log("공격 쿨타임!!!!!!!");
            return;
        }

        lastAttackTime = Time.time;

        // Shoot(self, dir);
        return;
    }
    
    private void Shoot(Transform self, Vector2 dir)
    {
        Debug.DrawRay(self.position, dir.normalized * attackRange, Color.cyan, 0.2f);

        DoHitscan(self, dir);
    }
    private void DoHitscan(Transform self, Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            self.position,
            dir.normalized,
            attackRange,
            LayerMask.GetMask("Water")
        );

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                target.ApplyKnockback(dir, 3f, 0.15f);
            }
        }
    }
}