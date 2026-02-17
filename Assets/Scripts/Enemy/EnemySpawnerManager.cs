using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<EnemyStats> resetStats;
    [SerializeField] private float timer;

    [SerializeField] private List<EnemyStats> _runTimeStats;

    private void Start()
    {
        for (int i = 0; i < resetStats.Count; i++)
            _runTimeStats[i] = Instantiate(resetStats[i]);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 15f)
        {
            timer = 0f;
            
        }
    }
}
