using UnityEngine;

public class Testmanager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private EnemyStats resetStat;
    [SerializeField] private float timer;

    private EnemyStats _runTimeStat;

    private void Start()
    {
        _runTimeStat = Instantiate(resetStat);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 10)
        {
            _runTimeStat.speed = 6.05f;
            enemy.GetComponent<Enemy>().stats = _runTimeStat;
            enemy.SetActive(true);
        }
        else if (timer > 5)
        {
            enemy.SetActive(false);
        }
    }
}
