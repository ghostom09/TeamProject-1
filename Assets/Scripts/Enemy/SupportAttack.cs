using UnityEngine;

public class SupportAttack : IEnemyAttackStrategy
{
    private float attackRange;
    private float attackSpeed;
    private float lastAttackTime;

    private float healPersent = 0.15f;
    private float interval;

    public void Init(EnemyStats stats, EnemyMove move)
    {
        attackRange = stats.attackRange;
        attackSpeed = stats.attackSpeed;
    }

    public void TryAttack(GameObject self, Transform target, Vector2 direction)
    {
        
        interval = 1f / attackSpeed;
        
        if (Time.time < lastAttackTime + interval)
            return;

        lastAttackTime = Time.time;
        
        Collider2D[] enemys = Physics2D.OverlapCircleAll(
            self.transform.position,
            attackRange,
            LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in enemys)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent == null)
                continue;

            if (enemyComponent.enemyStats.enemyType != EnemyType.support)
            {
                EnemyHit enemyHitComponent = enemy.GetComponent<EnemyHit>();
                if (enemyHitComponent.health * (1 + healPersent) >=
                    enemyHitComponent.maxHealth)
                {
                    enemyHitComponent.health = enemyHitComponent.maxHealth;
                }
                else
                {
                    enemyHitComponent.health += enemyHitComponent.maxHealth * healPersent;
                }
                
            }
        }
    }
}