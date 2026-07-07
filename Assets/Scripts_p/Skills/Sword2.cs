using System;
using UnityEngine;

public class Sword2 : SkillBase
{
    private LayerMask enemyLayer;

    private const float AngleRange = 125f;
    private float hitRadius;

    public Sword2()
    {
        enemyLayer = LayerMask.GetMask("Enemy", "Bullet");
    }

    protected override bool Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (player.isUsingUltimate)
            return false;
        
        hitRadius = data.Range * 1.5f;
        
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

        dir = dir.normalized;

        Vector2 origin = player.transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            hitRadius,
            enemyLayer
        );

        float halfAngle = AngleRange * 0.5f;

        foreach (var hit in hits)
        {
            Vector2 toTarget =
                ((Vector2)hit.transform.position - origin).normalized;

            float angle = Vector2.Angle(dir, toTarget);

            if (angle > halfAngle)
                continue;

            if (hit.TryGetComponent(out IDamageable target))
            {
                float finalDamage = player.Stats.Damage * 1.8f;

                target.TakeDamage(finalDamage);
                target.ApplyKnockback(dir, 8f, 0.15f);

                player.AddGauge(1);
            }
        }

        // 환영 생성
        playerAttack.SpawnIllusions(player.Stats.Damage * 0.35f);

        return true;
    }
}
