using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject target;

    [SerializeField] private List<EnemyStats> resetStats;
    [SerializeField] private List<EnemyStats> _runTimeStats = new();

    private List<Transform> _spawnPoints = new List<Transform>();

    [SerializeField] private float tankerMaxProb = 0.25f;
    [SerializeField] private float rangedMaxProb = 0.25f;
    [SerializeField] private float supportMaxProb = 0.15f;

    private float tankerProb = 0f;
    private float rangedProb = 0f;
    private float supportProb = 0f;

    [SerializeField] private float enemyProb = 0.001f;
    [SerializeField] private float enemyMaxProb = 0.006f;

    private int normalIndex;
    private int rangedIndex;
    private int tankerIndex;
    private int supportIndex;
    
    private Queue<Enemy> pool = new Queue<Enemy>();
    
    [SerializeField] private Transform poolParent;
    [SerializeField] private int initialPoolSize = 30;
    
    [SerializeField] private int maxActiveEnemy = 30;
    [SerializeField] private int ActiveEnemy = 0;

    private void Awake()
    {
        InitializeStats();
        InitializeSpawnPoints();
        InitPool();
    }
    
    private void InitPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, poolParent);
            obj.SetActive(false);

            pool.Enqueue(obj.GetComponent<Enemy>());
        }
    }
    
    public void ReturnToPool(Enemy enemy)
    {
        ActiveEnemy--;
        enemy.gameObject.SetActive(false);
        pool.Enqueue(enemy);
    }

    private void InitializeStats()
    {
        _runTimeStats.Clear();

        for (int i = 0; i < resetStats.Count; i++)
        {
            EnemyStats clone = Instantiate(resetStats[i]);
            _runTimeStats.Add(clone);

            switch (resetStats[i].enemyType)
            {
                case EnemyType.normal:
                    normalIndex = i;
                    break;
                case EnemyType.ranged:
                    rangedIndex = i;
                    break;
                case EnemyType.tanker:
                    tankerIndex = i;
                    break;
                case EnemyType.support:
                    supportIndex = i;
                    break;
            }
        }
    }

    private void InitializeSpawnPoints()
    {
        _spawnPoints.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            _spawnPoints.Add(transform.GetChild(i));
            _spawnPoints[i].GetComponent<EnemySpawner>().Init(this);
        }
    }

    public void ApplyDifficulty(int level)
    {
        DifficultyModifier mod = DifficultyCalculator.Calculate(level);

        ApplyStats(mod);
        ApplySpawnProbability(mod);
    }

    private void ApplyStats(DifficultyModifier mod)
    {
        for (int i = 0; i < resetStats.Count; i++)
        {
            EnemyStats baseStat = resetStats[i];
            EnemyStats runtime = _runTimeStats[i];

            runtime.health = baseStat.health * mod.healthMultiplier;
            runtime.damage = baseStat.damage * mod.damageMultiplier;
        }
    }

    private void ApplySpawnProbability(DifficultyModifier mod)
    {
        tankerProb = Mathf.Min(
            mod.tankerSpawnPercent + mod.specialSpawnPercent,
            tankerMaxProb);

        rangedProb = Mathf.Min(
            mod.rangedSpawnPercent + mod.specialSpawnPercent,
            rangedMaxProb);

        supportProb = Mathf.Min(
            mod.supportSpawnPercent + mod.specialSpawnPercent,
            supportMaxProb);

        enemyProb = Mathf.Min(
            enemyProb + mod.enemySpawnPercent,
            enemyMaxProb);
    }
    

    public bool CanSpawn()
    {
        return Random.value <= enemyProb;
    }

    public EnemyStats GetRandomEnemy()
    {
        float rand = Random.value;

        if (rand < tankerProb)
            return _runTimeStats[tankerIndex];

        rand -= tankerProb;

        if (rand < rangedProb)
            return _runTimeStats[rangedIndex];

        rand -= rangedProb;

        if (rand < supportProb)
            return _runTimeStats[supportIndex];

        return _runTimeStats[normalIndex];
    }

    public void SpawnFromPoint(Vector3 position)
    {
        if(ActiveEnemy>=maxActiveEnemy)
            return;
        Enemy enemy;

        if (pool.Count > 0)
        {
            enemy = pool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(enemyPrefab, poolParent);
            enemy = obj.GetComponent<Enemy>();
        }

        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
        
        EnemyStats stats = GetRandomEnemy();

        enemy.Init(stats, target, this);

        ActiveEnemy++;
    }
}