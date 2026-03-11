using UnityEngine;

public class WarriorNormalAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private float distSqr;
    
    private BossAttack bossAttack;
        
    public void Init(BossSkills data, BossAttack bossAttack)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, Transform target, Vector2 direction)
    {
        distSqr = (target.position - boss.transform.position).sqrMagnitude;

        if (distSqr > attackRange * attackRange)
            return;

        target.GetComponent<IDamageable>()?.TakeDamage(damage);
        bossAttack.EndCast();
    }
}
