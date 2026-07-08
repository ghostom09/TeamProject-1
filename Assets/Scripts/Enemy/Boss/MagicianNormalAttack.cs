using UnityEngine;
using System.Collections;

public class MagicianNormalAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private Vector2 dir;
    private BossAttack bossAttack;
    private const float CastDelay = 0.3f;
    
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(AttackRoutine(boss, target, onComplete));
    }

    private IEnumerator AttackRoutine(GameObject boss, GameObject target , System.Action onComplete)
    {
        bossAttack.SpawnMagicCircle(BossSkillType.Normal, boss.transform.position, 1.2f, CastDelay);
        yield return new WaitForSeconds(CastDelay);
        
        dir = (target.transform.position - boss.transform.position).normalized;
        bossAttack.ShootProjectile(dir, damage, attackRange);

        EndAttack(null, onComplete);
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
