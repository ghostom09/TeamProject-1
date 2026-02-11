using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Enemy enemyStat;
    
    private float damage;
    private float attackRange;
    private float attackSpeed;
    
    private void Awake()
    {
        enemyStat = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        damage = enemyStat.stats.damage;
        attackRange = enemyStat.stats.attackRange;
        attackSpeed = enemyStat.stats.attackSpeed;
    }
}
