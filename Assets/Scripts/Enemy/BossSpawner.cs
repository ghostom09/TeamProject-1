using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    private EnemySpawnerManager manager;

    public void Init(EnemySpawnerManager m)
    {
        manager = m;
    }

    private void FixedUpdate()
    {
        if (manager == null) return;

        if (manager.CanSpawnBoss())
        {
            manager.BossSpawnFromPoint(transform.position);
        }
    }
}
