using UnityEngine;
using System.Collections;

public class WarriorUltimateAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private float distSqr;
    
    private Vector2 dir;
    private BossAttack bossAttack;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss,  target, onComplete));
    }
    private IEnumerator Attack(GameObject boss, GameObject target, System.Action onComplete)
    {
        yield return new WaitForSeconds(3f);
        
        dir = (target.transform.position - boss.transform.position).normalized;
        
        yield return new WaitForSeconds(0.5f);
        
        bossAttack.ShootUltimateProjectile(dir, damage, attackRange);

        EndAttack(null, onComplete);
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
