using System.Collections.Generic;
using UnityEngine;

public class SwordNormalAttack : INormalAttack
{
    private int comboIndex = 0;
    private float lastAttackTime;
    private const float ComboResetTime = 1.5f;

    private CharacterData data;

    private LayerMask enemyLayer;

    public void Init(CharacterData data)
    {
        this.data = data;
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private float GetComboRangeMultiplier()
    {
        return comboIndex switch
        {
            0 => 0.8f,
            1 => 1.0f,
            2 => 1.2f,
            _ => 1f
        };
    }

    private float GetComboDamageMultiplier()
    {
        return comboIndex switch
        {
            0 => 0.5f,
            1 => 0.7f,
            2 => 1.2f,
            _ => 1f
        };
    }

    public bool TryAttack(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (Time.time > lastAttackTime + ComboResetTime)
            comboIndex = 0;

        float attackInterval = 1f / player.Stats.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
            return false;

        lastAttackTime = Time.time;

        DoComboAttack(player, user, dir);

        return true;
    }

    private void DoComboAttack(Player player, GameObject user, Vector2 dir)
    {
        Vector2 origin = user.transform.position;

        float range = data.Range * GetComboRangeMultiplier();
        float damage = player.Stats.Damage * GetComboDamageMultiplier();

        Vector2 center = origin + dir * range * 0.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            range * 0.5f,
            enemyLayer
        );

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable target))
                continue;

            if (hitTargets.Contains(target))
                continue;

            Vector2 toTarget =
                ((Vector2)hit.transform.position - origin).normalized;

            if (Vector2.Dot(dir, toTarget) < 0.3f)
                continue;

            target.TakeDamage(damage);
            target.ApplyKnockback(toTarget, 3f, 0.15f);

            hitTargets.Add(target);
        }

        comboIndex = (comboIndex + 1) % 3;
    }
}