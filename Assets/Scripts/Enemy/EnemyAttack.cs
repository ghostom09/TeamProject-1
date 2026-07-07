using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour, IEnemyReset
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    [SerializeField] private Animator animator;

    private IEnemyAttackStrategy strategy;

    private Transform attackTarget;
    private EnemyMove _enemyMove;

    private Vector2 dir;
    private bool hasAttackParam;

    private void Awake()
    {
        _enemyMove = GetComponent<EnemyMove>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        hasAttackParam = HasAnimatorParameter(AttackHash);
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
        if (strategy != null && strategy.TryAttack(gameObject, attackTarget, dir))
            SetAttackTrigger();
    }

    private void SetAttackTrigger()
    {
        if (animator != null && hasAttackParam)
            animator.SetTrigger(AttackHash);
    }

    private bool HasAnimatorParameter(int parameterHash)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash)
                return true;
        }

        return false;
    }
}
