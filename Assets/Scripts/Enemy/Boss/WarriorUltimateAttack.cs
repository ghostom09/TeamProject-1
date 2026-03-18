using UnityEngine;
using System.Collections;

public class WarriorUltimateAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private float distSqr;
    
    private BossAttack bossAttack;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject hitArea, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss,  target));
    }
    private IEnumerator Attack(GameObject boss, GameObject target)
    {
        distSqr = ((target.transform.position.x - boss.transform.position.x) *
                   (target.transform.position.x - boss.transform.position.x)) +
                  ((target.transform.position.y - boss.transform.position.y) *
                   (target.transform.position.y - boss.transform.position.y));;

        if (distSqr > attackRange * attackRange)
            yield break;

        yield return new WaitForSeconds(4f);
        
        yield return null;
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
