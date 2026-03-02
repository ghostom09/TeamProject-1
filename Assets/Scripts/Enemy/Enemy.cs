using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemyReset
{
    private EnemyMove move;
    private EnemyAttack attack;
    private EnemyHit hit;
    
    public EnemyStats enemyStats;
    
    private void Awake()
    {
        move = GetComponent<EnemyMove>();
        attack = GetComponent<EnemyAttack>();
        hit = GetComponent<EnemyHit>();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        enemyStats = stats;
        
        move.Init(enemyStats, target, m);
        attack.Init(enemyStats, target, m);
        hit.Init(enemyStats, target, m);
        
        if(enemyStats.enemyType == EnemyType.support)
            GetComponent<SpriteRenderer>().color = Color.blue;
        else if(enemyStats.enemyType == EnemyType.ranged)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if(enemyStats.enemyType == EnemyType.tanker)
            GetComponent<SpriteRenderer>().color = Color.yellow;
    }
}
