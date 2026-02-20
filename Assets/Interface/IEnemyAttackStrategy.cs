using UnityEngine;

public interface IEnemyAttackStrategy
{
    void Init(EnemyStats stats);
    void TryAttack(GameObject self, Transform target, Vector2 direction);
}