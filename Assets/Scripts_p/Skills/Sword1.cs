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
        
        

        if (player.isUsingUltimate)
            return;
        
        dir = dir.normalized;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            user.transform.position,
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

        if (bestTarget == null)
        {
            Debug.Log("신법 실패: 적 없음");
            return;
        }

        Vector2 enemyPos = bestTarget.transform.position;
        user.transform.position = enemyPos;

        // 데미지 처리
        if (bestTarget.TryGetComponent(out IDamageable target) && user.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            target.TakeDamage(data.Damage * 1.2f);
            target.ApplyKnockback(dir, 6f, 0.15f);
            player.AddGauge(1);
            damageable.ApplyKnockback(-dir, 10f, 0.15f);
        }
        
        playerAttack.FireIllusions(bestTarget.transform, user);
        
        lastUsedTime = Time.time;
    }
}
