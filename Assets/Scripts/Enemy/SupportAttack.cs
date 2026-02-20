using UnityEngine;

public class SupportAttack : IEnemyAttackStrategy
{
    private float attackRange;
    private float attackSpeed;
    private float lastAttackTime;

    public void Init(EnemyStats stats)
    {
        attackRange = stats.attackRange;
        attackSpeed = stats.attackSpeed;
    }

    public void TryAttack(GameObject self, Transform target, Vector2 direction)
    {
        
    }
}