using UnityEngine;

public interface IBossReset
{
    void Init(BossStats stats, GameObject target, EnemySpawnerManager m);
}
