using System.Collections;
using UnityEngine;

public class Sword1 : SkillBase
{
    private LayerMask enemyLayer;

    public Sword1()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();
        PlayerAttack playerAttack = user.GetComponent<PlayerAttack>();
        Rigidbody2D rb = user.GetComponent<Rigidbody2D>();

        if (player.isUsingUltimate)
            return;

        dir = dir.normalized;

        Vector2 origin = rb.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            data.Range * 3,
            enemyLayer
        );

        Collider2D bestTarget = null;
        float bestAngle = 30f;

        foreach (var h in hits)
        {
            Vector2 toTarget = (h.transform.position - user.transform.position).normalized;
            float angle = Vector2.Angle(dir, toTarget);

            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestTarget = h;
            }
        }

        if (bestTarget != null)
        {
            Vector2 enemyPos = bestTarget.transform.position;

            player.StartCoroutine(FastTeleport(rb, enemyPos, 0.06f));

            if (bestTarget.TryGetComponent(out IDamageable target) &&
                user.TryGetComponent(out IDamageable damageable))
            {
                target.TakeDamage(data.Damage * 1.2f);
                target.ApplyKnockback(dir, 6f, 0.15f);
                player.AddGauge(1);

                damageable.ApplyKnockback(-dir, 10f, 0.15f);
            }
            
            player.StartInvincibility(0.5f);
            playerAttack.FireIllusions(bestTarget.transform, user);
        }
        else
        {
            float teleportDistance = data.Range * 2;
            Vector2 teleportPos = origin + dir * teleportDistance;

            player.StartCoroutine(FastTeleport(rb, teleportPos, 0.06f));

            Debug.Log("신법: 적 없음 → 방향 텔포");
        }

        lastUsedTime = Time.time;
    }
    
    private IEnumerator FastTeleport(Rigidbody2D rb, Vector2 targetPos, float duration)
    {
        float elapsed = 0f;
        Vector2 startPos = rb.position;

        // 이동 중 물리 충돌 방지 원하면 활성화
        rb.linearVelocity = Vector2.zero;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = elapsed / duration;
            
            t = 1 - Mathf.Pow(1 - t, 3);

            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
    }
}