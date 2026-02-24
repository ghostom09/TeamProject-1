using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemyReset
{
    private EnemyMove move;
    private EnemyAttack attack;
    private EnemyHit hit;
    
    private void Awake()
    {
        move = GetComponent<EnemyMove>();
        attack = GetComponent<EnemyAttack>();
        hit = GetComponent<EnemyHit>();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        move.Init(stats, target, m);
        attack.Init(stats, target, m);
        hit.Init(stats, target, m);
    }
}
