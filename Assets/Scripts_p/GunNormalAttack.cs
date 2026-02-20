using UnityEngine;

public class GunNormalAttack : INormalAttack
{
    private float lastAttackTime;
    private CharacterData data;

    private bool enhanced;

    public void Init(CharacterData data)
    {
        this.data = data;
    }

    public void SetEnhancedMode(bool value)
    {
        enhanced = value;
        Debug.Log("변함");
    }

    public bool TryAttack(GameObject user, Vector2 dir, GameObject hitBox)
    {
        float attackInterval = 1f / data.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
            return false;

        lastAttackTime = Time.time;

        Shoot(user, dir);
        return true;
    }

    public void EndAttack(GameObject user, GameObject hitBox)
    {
        // 총은 히트박스 안 쓰니까 비워둬도 됨
    }

    private void Shoot(GameObject user, Vector2 dir)
    {
        float damage = enhanced ? data.Damage * 2.2f : data.Damage;
        float range = data.Range;

        Debug.DrawRay(user.transform.position, dir.normalized * range, Color.cyan, 0.2f);

        if (enhanced)
            DoPiercingHitscan(user, dir, damage, range);
        else
            DoSingleHitscan(user, dir, damage, range);
    }
    
    private void DoSingleHitscan(GameObject user, Vector2 dir, float damage, float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            user.transform.position,
            dir.normalized,
            range,
            LayerMask.GetMask("Enemy")
        );

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                target.ApplyKnockback(dir, 3f, 0.15f);
            }
        }
    }
    private void DoPiercingHitscan(GameObject user, Vector2 dir, float damage, float range)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            user.transform.position,
            dir.normalized,
            range * 2,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                user.GetComponent<Player>().AddGauge(1);
                // 강화 모드는 약한 넉백
                target.ApplyKnockback(dir, 2f, 0.08f);
            }
        }
    }
}
