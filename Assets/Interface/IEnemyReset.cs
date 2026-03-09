using UnityEngine;

public interface IEnemyReset
{
    void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m);
}
