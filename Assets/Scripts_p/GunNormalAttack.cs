using UnityEngine;

public class GunNormalAttack : INormalAttack
{
    private float lastAttackTime;
    private CharacterData data;

    public void Init(CharacterData data)
    {
        this.data = data;
    }

    public bool TryAttack(GameObject user, Vector2 dir, GameObject hitBox)
    {
        float attackInterval = 1f / data.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
        {
            Debug.Log("공격 쿨타임!!!!!!!");
            return false;
        }

        lastAttackTime = Time.time;

        Shoot(user, dir, hitBox);
        return true;
    }

    public void EndAttack(GameObject user, GameObject hitBox)
    {
        
    }

    private void Shoot(GameObject user, Vector2 dir, GameObject hitBox)
    {
        float damage = data.Damage;
        float range = data.Range;

        Debug.Log($"공격력 : {damage}, 사거리 : {range}");

        // 시각 디버그
        Debug.DrawRay(user.transform.position, dir.normalized * range, Color.cyan, 0.2f);

        // 히트스캔 판정
        DoHitscan(user, dir, damage, range);

        // 3. 나중 
        // ApplyRecoil();
        // PlayMuzzleEffect();
    }
    private void DoHitscan(GameObject user, Vector2 dir, float damage, float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            user.transform.position,
            dir.normalized,
            range,
            LayerMask.GetMask("Water")
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
}