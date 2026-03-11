using UnityEngine;

public class Boss : MonoBehaviour, IBossReset
{
    private BossMove move;
    private BossAttack attack;
    private BossHit hit;
    [SerializeField] private GameObject targetObj;
    [SerializeField] private EnemySpawnerManager manager;

    [SerializeField] private BossStats bossStats;
    
    private void Start()
    {
        move = GetComponent<BossMove>();
        attack = GetComponent<BossAttack>();
        hit = GetComponent<BossHit>();
        
        Init(bossStats, targetObj, manager);
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        bossStats = stats;
        
        move.Init(bossStats, target, m);
        attack.Init(bossStats, target, m);
        hit.Init(bossStats, target, m);
    }
}
