using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemySpawnerManager manager;

    public void Init(EnemySpawnerManager m)
    {
        manager = m;
    }

    private void FixedUpdate()
    {
        if (manager == null) return;

        if (manager.CanSpawn())
        {
            manager.SpawnFromPoint(transform.position, false);
        }
    }
}