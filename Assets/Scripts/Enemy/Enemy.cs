using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;

    private void OnEnable()
    {
        GetComponent<EnemyMove>().Init(stats);
    }
}
