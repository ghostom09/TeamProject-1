using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private EnemyStats resetStat;
    [SerializeField] private float timer;

    [SerializeField] private EnemyStats _runTimeStat;

    private void Start()
    {
        _runTimeStat = Instantiate(resetStat);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 10)
        {
            _runTimeStat.jumpForce = 40f;
            enemy.GetComponent<Enemy>().stats = _runTimeStat;
            enemy.SetActive(true);
        }
        else if (timer > 5)
        {
            enemy.SetActive(false);
        }
    }
}
