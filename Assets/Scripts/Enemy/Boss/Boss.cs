using UnityEngine;

public class Boss : MonoBehaviour, IBossReset
{
    private BossMove move;
    private BossAttack attack;
    private BossHit hit;
    private GameObject targetObj;
    public EnemySpawnerManager manager;

    private BossStats bossStats;
    
    private void Awake()
    {
        move = GetComponent<BossMove>();
        attack = GetComponent<BossAttack>();
        hit = GetComponent<BossHit>();
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        bossStats = stats;
        manager = m;
        targetObj = target;
        
        move.Init(bossStats, targetObj, manager);
        attack.Init(bossStats, targetObj, manager);
        hit.Init(bossStats, targetObj, manager);
    }
}
