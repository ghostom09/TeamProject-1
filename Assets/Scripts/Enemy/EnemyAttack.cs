using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Enemy enemy;
    private IEnemyAttackStrategy strategy;

    [SerializeField] private Transform target;

    private Vector2 dir;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        strategy = enemy.stats.enemyType switch
        {
            EnemyType.Normal  => new MeleeAttack(),
            EnemyType.tanker  => new MeleeAttack(),
            EnemyType.Ranged  => new RangedAttack(),
            EnemyType.suport  => new SupportAttack(),
            _ => null
        };

        strategy?.Init(enemy.stats);
    }

    private void Update()
    {
        dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        strategy?.TryAttack(gameObject, target, dir);
    }
}