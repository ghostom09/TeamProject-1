using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    private Enemy enemy;
    private IEnemyAttackStrategy strategy;

    [SerializeField] private Transform player;

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
        strategy?.TryAttack(transform, player);
    }
}