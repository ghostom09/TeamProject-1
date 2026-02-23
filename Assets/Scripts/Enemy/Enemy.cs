using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    private GameObject target;

    private void OnEnable()
    {
        GetComponent<EnemyMove>().Init(stats, target);
        GetComponent<EnemyAttack>().Init(stats, target);
    }
}
