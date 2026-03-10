using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordNormalAttack : INormalAttack
{
    private int comboIndex = 0;
    private float lastAttackTime;
    private float comboResetTime = 1.5f;

    private CharacterData data;

    public void Init(CharacterData data)
    {
        this.data = data;
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
        if (Time.time > lastAttackTime + comboResetTime)
            comboIndex = 0;

        float attackInterval = 1f / data.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
        {
            Debug.Log("공격 쿨타임!!!");
            return false;
        }

        lastAttackTime = Time.time;

        DoComboAttack(user, dir);

        return true;
    }
    private void DoComboAttack(GameObject user, Vector2 dir)
    {
        float range = data.Range * GetComboRangeMultiplier();
        float damage = data.Damage * GetComboDamageMultiplier();

        Vector2 center =
            (Vector2)user.transform.position +
            dir * range * 0.5f;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(center, range * 0.5f, LayerMask.GetMask("Enemy"));

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                if (hitTargets.Contains(target))
                    continue;

                Vector2 toTarget =
                    (hit.transform.position - user.transform.position).normalized;

                if (Vector2.Dot(dir, toTarget) < 0.3f)
                    continue;

                target.TakeDamage(damage);
                target.ApplyKnockback(toTarget, 3f, 0.15f);

                hitTargets.Add(target);
            }
        }

        comboIndex = (comboIndex + 1) % 3;
    }
}
