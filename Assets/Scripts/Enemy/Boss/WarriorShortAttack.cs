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
    private Vector2 boxOffset;

    private GameObject hitArea;
    
    private Vector2 dir;

    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject hitArea, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
        targetLayer = 1 << target.layer;
        
        this.hitArea = hitArea;

        boxSize = new Vector2(attackRange*2, boss.transform.localScale.y/2);
        boxOffset = new Vector2(0f, -boss.transform.localScale.y / 4);
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(Attack(boss, target, direction, onComplete));
    }

    private IEnumerator Attack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        Debug.Log("단거리 공격 시작");
        
        Vector2 boxCenter = (Vector2)boss.transform.position + boxOffset;
        
        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        
        hitArea.transform.position = boxCenter;
        hitArea.transform.localScale = boxSize;
        hitArea.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);
        
        Collider2D[] hits = 
            Physics2D.OverlapBoxAll(boxCenter, 
                boxSize, 
                0f, 
                targetLayer);
        
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable targetComponent))
                continue;
            
            if (hitTargets.Contains(targetComponent))
                continue;
            
            float directionSign = hit.transform.position.x - boss.transform.position.x > 0 ? 1f : -1f;
            
            dir = new Vector2(directionSign * 0.258f, 0.966f).normalized;;
            targetComponent.ApplyKnockback(dir, 15f, 0.2f);
            targetComponent.TakeDamage(damage);
            
            hitTargets.Add(targetComponent);
        }

        EndAttack(hitArea, onComplete);
        yield return null;
    }

    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
        hitArea.SetActive(false);
    }
}