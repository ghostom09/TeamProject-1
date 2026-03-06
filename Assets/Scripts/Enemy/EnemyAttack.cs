using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour, IEnemyReset
{
    private IEnemyAttackStrategy strategy;

    private Transform attackTarget;
    private EnemyMove _enemyMove;

    private Vector2 dir;

    private void Awake()
    {
        _enemyMove = GetComponent<EnemyMove>();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        strategy = stats.enemyType switch
        {
            EnemyType.normal  => new MeleeAttack(),
            EnemyType.tanker  => new MeleeAttack(),
            EnemyType.ranged  => new RangedAttack(),
            EnemyType.support  => new SupportAttack(),
            _ => null
        };

        attackTarget = target.transform;
        
        strategy?.Init(stats, _enemyMove);
    }

    private void Update()
    {
        if(attackTarget == null)
            return;
        dir = ((Vector2)attackTarget.position - (Vector2)transform.position).normalized;
        strategy?.TryAttack(gameObject, attackTarget, dir);
    }
}