using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarriorLongAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private LayerMask targetLayer;
    
    private const float ConeThreshold = 0.3f;
    
    private Vector2 dir;
    private BossAttack bossAttack;
        
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
        targetLayer = 1 << target.layer;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss, target, direction, onComplete));
    }

    private IEnumerator Attack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        Debug.Log("Warrior wide melee attack start");
        const float attackDelay = 2f;
        const float effectDelay = attackDelay * 0.5f;

        yield return new WaitForSeconds(effectDelay);

        Vector2 effectPosition = (Vector2)boss.transform.position + direction.normalized * (attackRange * 0.5f);
        bossAttack?.SpawnWarriorAttackEffect(
            BossSkillType.longDistance,
            effectPosition,
            direction,
            new Vector2(attackRange * 1.25f, attackRange * 1.25f),
            attackDelay - effectDelay);

        yield return new WaitForSeconds(attackDelay - effectDelay); 

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
            GameResultTracker.Instance?.SetDeathReason("Hit by Warrior Boss sword wave");
            targetComponent.TakeDamage(damage);

            hitTargets.Add(targetComponent);
        }

        EndAttack(null, onComplete);
    }

    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        Debug.Log("Warrior wide melee attack end");
        onComplete?.Invoke();
    }
}
