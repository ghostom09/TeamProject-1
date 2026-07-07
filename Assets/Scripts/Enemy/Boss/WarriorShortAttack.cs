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

    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
        targetLayer = 1 << target.layer;
        boxSize = new Vector2(attackRange, Mathf.Max(0.5f, boss.transform.localScale.y / 2f));
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss, target, direction, onComplete));
    }

    private IEnumerator Attack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        Debug.Log("단거리 공격 시작");
        
        Vector2 attackDirection = direction.sqrMagnitude > 0.001f
            ? direction.normalized
            : Vector2.right;
        Vector2 boxCenter = (Vector2)boss.transform.position + attackDirection * (attackRange * 0.5f);
        float boxAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        
        yield return new WaitForSeconds(1.5f);

        Vector2 effectPosition = boxCenter + Vector2.down * (boxSize.y * 0.5f);

        bossAttack?.SpawnWarriorAttackEffect(
            BossSkillType.shortDistance,
            effectPosition,
            Vector2.right,
            boxSize,
            1f);
        
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
