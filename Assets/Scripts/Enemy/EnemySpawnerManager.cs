using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject target;
    [SerializeField] private Transform poolParent;

    [SerializeField] private List<EnemyStats> resetStats;
    [SerializeField] private List<EnemyStats> _runTimeStats = new();
    [SerializeField] private List<BossStats> resetBossStats;
    [SerializeField] private List<BossStats> _runTimeBossStats = new();

    private List<Transform> _spawnPoints = new List<Transform>();
    private Transform _bossSpawnPoint;

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
    
    private int warriorIndex;
    private int magicianIndex;
    
    private bool isBossActive = false;
    private bool isBossPending = false;
    private int currentBossTypeIndex = 0;
    private Boss currentBossInstance;
    
    private Queue<Enemy> pool = new Queue<Enemy>();
    
    [SerializeField] private int initialPoolSize = 30;
    
    [SerializeField] private int maxActiveEnemy = 30;
    [SerializeField] private int activeEnemyLimit = 4;
    [SerializeField] private int ActiveEnemy = 0;
    
    private GameObject bossObj;

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
        bossObj = Instantiate(bossPrefab, transform.position, Quaternion.identity, poolParent);
        bossObj.SetActive(false);
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
        
        for (int i = 0; i < resetBossStats.Count; i++)
        {
            BossStats clone = Instantiate(resetBossStats[i]);
            _runTimeBossStats.Add(clone);

            switch (resetBossStats[i].bossType)
            {
                case BossType.warrior:
                    warriorIndex = i;
                    break;
                case BossType.magician:
                    magicianIndex = i;
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
            _spawnPoints[i].GetComponent<EnemySpawner>()?.Init(this);
        }
        
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out BossSpawner target))
            {
                _bossSpawnPoint = target.transform;
                target.Init(this);
            }
        }
    }

    public void ApplyDifficulty(int level)
    {
        DifficultyModifier mod = DifficultyCalculator.Calculate(level);

        ApplyStats(mod);
        ApplySpawnProbability(mod);

        activeEnemyLimit = Mathf.Min(
            mod.enemyLimit,
            maxActiveEnemy);
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
    
    public void SetBossPending()
    {
        if (!isBossActive)
        {
            isBossPending = true;
        }
    }
    
    public bool CanSpawnBoss()
    {
        return isBossPending && !isBossActive;
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

    public void SpawnFromPoint(Vector3 position, bool boss)
    {
        if(ActiveEnemy>=activeEnemyLimit && !boss)
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
        if (boss)
            stats = _runTimeStats[normalIndex];

        enemy.Init(stats, target, this);

        ActiveEnemy++;
    }

    public void BossSpawnFromPoint(Vector3 position)
    {
        if (isBossActive) return;

        bossObj.SetActive(true);
        bossObj.transform.position = position;
        
        Boss bossScript = bossObj.GetComponent<Boss>();
        
        BossStats selectedBossStat = _runTimeBossStats[currentBossTypeIndex%_runTimeBossStats.Count];
        currentBossTypeIndex++;
        
        bossScript.Init(selectedBossStat, target, this);

        isBossActive = true;
        isBossPending = false;
        currentBossInstance = bossScript;

        Debug.Log($"보스 출현! 타입: {selectedBossStat.bossType}");
    }

    public void BossDie()
    {
        bossObj.SetActive(false);
        isBossActive = false;
    }
}