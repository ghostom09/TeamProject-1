using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorLongAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private float distSqr;
    private LayerMask targetLayer;
    
    private const float ConeThreshold = 0.5f;
    
    private BossAttack bossAttack;
    
    private GameObject hitArea;
    
    private Vector2 dir;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject hitArea, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
        targetLayer = 1<<target.layer;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss, target, direction, onComplete));
    }

    private IEnumerator Attack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        Debug.Log("장거리공격 시작");
        yield return new WaitForSeconds(2f); 

        Collider2D[] hits = 
            Physics2D.OverlapCircleAll(boss.transform.position, 
                attackRange, 
                targetLayer);

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable targetComponent))
                continue;

            if (hitTargets.Contains(targetComponent))
                continue;

            Vector2 hitTarget = ((Vector2)hit.transform.position - (Vector2)boss.transform.position).normalized;

            if (Vector2.Dot(direction.normalized, hitTarget) < ConeThreshold)
                continue;
        
            
            dir = (target.transform.position - boss.transform.position).normalized;
            targetComponent.ApplyKnockback(dir, 8f, 0.25f);
            targetComponent.TakeDamage(damage);

            hitTargets.Add(targetComponent);
        }

        EndAttack(null, onComplete);
    }

    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        Debug.Log("장거리공격 끝");
        onComplete?.Invoke();
    }
}
