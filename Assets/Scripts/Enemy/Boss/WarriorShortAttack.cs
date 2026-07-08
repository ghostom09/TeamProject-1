using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorShortAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    private LayerMask targetLayer;
    
    private BossAttack bossAttack;
    
    private Vector2 boxSize;
    private Vector2 dir;
    private BoxCollider2D bossCollider;

    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
        targetLayer = 1 << target.layer;
        boxSize = new Vector2(attackRange+2, Mathf.Max(0.5f, boss.transform.localScale.y / 2f));
        bossCollider =  boss.GetComponent<BoxCollider2D>();
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss, target, direction, onComplete));
    }

    private IEnumerator Attack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        Debug.Log("단거리 공격 시작");
        
        yield return new WaitForSeconds(1.5f);
        
        Vector2 feetPosition;
        if (bossCollider != null)
        {
            feetPosition = new Vector2(bossCollider.bounds.center.x, bossCollider.bounds.min.y);
        }
        else
        {
            feetPosition = (Vector2)boss.transform.position;
        }

        Vector2 boxCenter = feetPosition + new Vector2(0, boxSize.y / 2f);
        float boxAngle = 0f;

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        
        bossAttack?.SpawnWarriorAttackEffect(
            BossSkillType.shortDistance,
            boxCenter,
            Vector2.left,
            boxSize,
            0.75f);
        
        Collider2D[] hits = 
            Physics2D.OverlapBoxAll(boxCenter, 
                boxSize, 
                boxAngle, 
                targetLayer);
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable targetComponent))
                continue;
            
            if (hitTargets.Contains(targetComponent))
                continue;
            
            dir = ((Vector2)hit.transform.position - (Vector2)boss.transform.position).normalized;
            targetComponent.ApplyKnockback(dir, 15f, 0.2f);
            GameResultTracker.Instance?.SetDeathReason("Hit by Warrior Boss ground slam");
            targetComponent.TakeDamage(damage);
            
            hitTargets.Add(targetComponent);
        }

        EndAttack(null, onComplete);
        yield return null;
    }

    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
