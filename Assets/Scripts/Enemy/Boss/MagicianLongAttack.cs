using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicianLongAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    private BossAttack bossAttack;
    private LineRenderer lineRenderer;
    private LayerMask targetLayer;

    private float lastDamageTime;

    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
        lineRenderer = boss.GetComponent<LineRenderer>();
        targetLayer = 1 << target.layer;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>()
            .StartCoroutine(AttackRoutine(boss, target, onComplete));
    }

    private IEnumerator AttackRoutine(GameObject boss, GameObject target, System.Action onComplete)
    {
        yield return new WaitForSeconds(0.5f);
        
        Vector2 dir = (target.transform.position - boss.transform.position).normalized;
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;

        float timer = 2f;
        lastDamageTime = -99f;

        while (timer > 0)
        {
            bool canApplyDamage = (Time.time >= lastDamageTime);
            
            UpdateLaser(boss, dir, canApplyDamage);

            if (canApplyDamage) lastDamageTime = Time.time;

            timer -= Time.deltaTime;
            yield return null;
        }

        lineRenderer.enabled = false;
        onComplete?.Invoke();
    }

    private void UpdateLaser(GameObject boss, Vector2 dir, bool shouldApplyDamage)
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            (Vector2)boss.transform.position + dir * (attackRange / 2f),
            new Vector2(attackRange, lineRenderer.startWidth),
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg,
            Vector2.zero,
            0f,
            targetLayer
        );

        Vector2 endPoint = (Vector2)boss.transform.position + dir * attackRange;

        if (hit.collider != null)
        {
            float distance = Vector2.Distance(hit.point, boss.transform.position);;
            endPoint = (Vector2)boss.transform.position + dir * distance;

            if (shouldApplyDamage)
            {
                var d = hit.collider.GetComponent<IDamageable>();
                if (d != null)
                {
                    d.TakeDamage(damage); 
                    d.ApplyKnockback(dir, 2f, 0.1f);
                }
            }
        }

        lineRenderer.SetPosition(0, boss.transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }

    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}