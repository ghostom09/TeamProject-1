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

    public bool TryAttack(GameObject self, Transform target, Vector2 direction)
    {
        
        interval = 1f / attackSpeed;
        
        if (Time.time < lastAttackTime + interval)
            return false;

        lastAttackTime = Time.time;
        
        DrawAttackRange(self.transform, 0.3f);
        
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

        return true;
    }
    
    private void DrawAttackRange(Transform self, float duration)
    {
        int segments = 40;
        float angleStep = 360f / segments;
        Vector3 prevPoint = self.position + Vector3.right * attackRange;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = self.position + new Vector3(
                Mathf.Cos(angle) * attackRange,
                Mathf.Sin(angle) * attackRange
            );

            Debug.DrawLine(prevPoint, newPoint, Color.cyan, duration);
            prevPoint = newPoint;
        }
    }
}
