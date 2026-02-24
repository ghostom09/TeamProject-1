using UnityEngine;

public interface IEnemyAttackStrategy
{
    void Init(EnemyStats stats, EnemyMove move);
    void TryAttack(GameObject self, Transform target, Vector2 direction);
}