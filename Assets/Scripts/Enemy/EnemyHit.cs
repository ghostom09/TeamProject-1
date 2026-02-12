using System;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    private Enemy enemyStat;
    
    private float health;
    
    private void Awake()
    {
        enemyStat = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        health = enemyStat.stats.health;
    }
}
