using System.Collections;
using UnityEngine;

public class Gun1 : SkillBase
{
    public bool ignoreMoveLock = false;

    private const float StopTime = 0.5f;
    private const float SlowPercent = 25f;
    private const float SlowDuration = 2.5f;

    private LayerMask hitLayer;

    public Gun1()
    {
        hitLayer = LayerMask.GetMask("Enemy", "Wall");
    }

    protected override bool Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (player.isUsingUltimate)
            return false;
        
        player.StartCoroutine(GoldenShotRoutine(player, dir));
        
        return true;
    }

    private IEnumerator GoldenShotRoutine(Player player, Vector2 dir)
    {
        IPlayerMover mover = player.GetComponent<IPlayerMover>();

        float waitTime = ignoreMoveLock ? 0f : StopTime;

        if (!ignoreMoveLock)
            mover?.SetMoveLock(MoveLockType.FullLock);

        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        mover?.SetMoveLock(MoveLockType.None);
        
        SkillController.Instance.Gun1(player.transform.position, dir);
        FireHitScan(player, dir);
    }

    private void FireHitScan(Player player, Vector2 dir)
    {
        Vector2 origin = player.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir.normalized,
            data.Range * 2,
            hitLayer
        );

        Debug.DrawRay(origin, dir.normalized * (data.Range * 2), Color.yellow, 1f);

        if (!hit) return;
        
        Collider2D[] explosion = Physics2D.OverlapCircleAll(
            hit.point,
            data.Range / 4,
            hitLayer
        );

        foreach (var col in explosion)
        {
            if (col.TryGetComponent<IDamageable>(out var target))
            {
                Vector2 knockDir =
                    (col.transform.position - (Vector3)hit.point).normalized;

                float finalDamage = player.Stats.Damage * 2.5f;

                target.TakeDamage(finalDamage);
                target.ApplyKnockback(knockDir, 6f, 0.15f);
                target.ApplySlow(SlowPercent, SlowDuration);

                player.AddGauge(1);
            }
        }
    }
}