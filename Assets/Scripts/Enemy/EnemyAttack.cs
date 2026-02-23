using UnityEngine;

public class EnemyAttack : MonoBehaviour, IEnemyReset
{
    private IEnemyAttackStrategy strategy;

    private Transform attackTarget;

    private Vector2 dir;


    public void Init(EnemyStats stats, GameObject target)
    {
        strategy = stats.enemyType switch
        {
            EnemyType.Normal  => new MeleeAttack(),
            EnemyType.tanker  => new MeleeAttack(),
            EnemyType.Ranged  => new RangedAttack(),
            EnemyType.suport  => new SupportAttack(),
            _ => null
        };

        attackTarget = target.transform;
        
        strategy?.Init(stats);
    }

    private void Update()
    {
        dir = ((Vector2)attackTarget.position - (Vector2)transform.position).normalized;
        strategy?.TryAttack(gameObject, attackTarget, dir);
    }
}