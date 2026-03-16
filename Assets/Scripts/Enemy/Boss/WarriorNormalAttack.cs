using UnityEngine;
using System.Collections;

public class WarriorNormalAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private float distSqr;
    
    private BossAttack bossAttack;
    
    private GameObject hitBox;
    private GameObject hitArea;
        
    public void Init(BossSkills data, BossAttack bossAttack, GameObject hitBox, GameObject hitArea)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss, target, onComplete));
    }

    private IEnumerator Attack(GameObject boss, GameObject target, System.Action onComplete)
    {
        distSqr = ((target.transform.position.x - boss.transform.position.x) *
                   (target.transform.position.x - boss.transform.position.x)) +
                  ((target.transform.position.y - boss.transform.position.y) *
                   (target.transform.position.y - boss.transform.position.y));

        if (distSqr > attackRange * attackRange)
        {
            EndAttack(hitBox, hitArea, onComplete);
            yield break;
        }

        yield return new WaitForSeconds(1f);
        target.GetComponent<IDamageable>()?.TakeDamage(damage);
        
        EndAttack(hitBox, hitArea, onComplete);
        
        yield return null;
    }

    public void EndAttack(GameObject hitBox, GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
