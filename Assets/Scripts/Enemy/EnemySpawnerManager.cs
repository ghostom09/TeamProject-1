using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<EnemyStats> resetStats;

    [SerializeField] private List<EnemyStats> _runTimeStats = new();

    private int normalIndex;
    private int rangedIndex;
    private int tankerIndex;
    private int supportIndex;

    [SerializeField] private float tankerMaxProb = 0.25f;
    [SerializeField] private float rangedMaxProb = 0.25f;
    [SerializeField] private float supportMaxProb = 0.15f;

    private float tankerProb = 0;
    private float rangedProb = 0;
    private float supportProb = 0;

    private void Start()
    {
        _runTimeStats.Clear();

        for (int i = 0; i < resetStats.Count; i++)
        {
            EnemyStats clone = Instantiate(resetStats[i]);
            _runTimeStats.Add(clone);
            switch (resetStats[i].enemyType)
            {
                case EnemyType.Normal:
                    normalIndex = i;
                    break;
                case EnemyType.Ranged:
                    rangedIndex = i;
                    break;
                case EnemyType.tanker:
                    tankerIndex = i;
                    break;
                case EnemyType.suport:
                    supportIndex = i;
                    break;
            }
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
}