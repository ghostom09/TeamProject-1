using UnityEngine;

public interface IEnemyAttackStrategy
{
    void Init(EnemyStats stats);
    void TryAttack(Transform self, Transform target);
}