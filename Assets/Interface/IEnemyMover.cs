using UnityEngine;

public interface IEnemyMover
{
    void Init(EnemyStats stats);
    
    void SetMoveLock(bool value);
}
