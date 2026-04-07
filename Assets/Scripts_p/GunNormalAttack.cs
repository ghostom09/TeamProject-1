using UnityEngine;

public class GunNormalAttack : INormalAttack
{
    private float lastAttackTime;
    private CharacterData data;

    private bool enhanced;

    private LayerMask hitLayer;

    public void Init(CharacterData data)
    {
        this.data = data;
        hitLayer = LayerMask.GetMask("Enemy", "Wall");
    }

    public void SetEnhancedMode(bool value)
    {
        enhanced = value;
        Debug.Log("변함");
    }

    public bool TryAttack(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        float attackInterval = 1f / player.Stats.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
            return false;

        lastAttackTime = Time.time;

        Shoot(player, user, dir.normalized);

        return true;
    }

    private void Shoot(Player player, GameObject user, Vector2 dir)
    {
        Vector2 origin = user.transform.position;

        float damage = enhanced ? player.Stats.Damage * 2.2f : player.Stats.Damage;
        float range = player.Stats.Range;

        if (enhanced)
            DoPiercingHitscan(origin, dir, damage, range);
        else
            DoSingleHitscan(origin, dir, damage, range);
    }

    private void DoSingleHitscan(Vector2 origin, Vector2 dir, float damage, float range)
    {
        SkillController.Instance.GunNormalAttack(origin, dir);
        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir,
            range,
            hitLayer
        );
        
        Debug.DrawRay(origin, dir * range, Color.cyan, 0.2f);

        if (hit.collider == null)
            return;

        if (hit.collider.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage);
            target.ApplyKnockback(dir, 3f, 0.15f);
        }
    }

    private void DoPiercingHitscan(Vector2 origin, Vector2 dir, float damage, float range)
    {
        SkillController.Instance.GunUltraAttack(origin, dir);
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            origin,
            dir,
            range * 2,
            hitLayer
        );
        Debug.DrawRay(origin, dir * range * 2, Color.cyan, 0.2f);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage);
                target.ApplyKnockback(dir, 2f, 0.08f);
            }
        }
    }
}