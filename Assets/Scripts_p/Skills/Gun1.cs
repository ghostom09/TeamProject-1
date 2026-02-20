using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Gun1 : SkillBase
{
    public bool ignoreMoveLock = false;
    
    private const float StopTime = 0.3f;
    private const float SlowPercent = 25f;
    private const float SlowDuration = 2.5f;

    private LayerMask hitLayer;
    

    public Gun1() : base(4f)
    {
        hitLayer = LayerMask.GetMask("Enemy");
    }

    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (player.isUsingUltimate)
            return;
        lastUsedTime = Time.time;
        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(GoldenShotRoutine(user, dir));
    }

    private IEnumerator GoldenShotRoutine(GameObject user, Vector2 dir)
    {
        // 1. 플레이어 정지
        var mover = user.GetComponent<IPlayerMover>();
        if (!ignoreMoveLock)
            mover?.SetMoveLock(true);

        yield return new WaitForSeconds(StopTime);

        mover?.SetMoveLock(false);

        // 2. 히트스캔 발사
        FireHitScan(user, dir);
    }

    private void FireHitScan(GameObject user, Vector2 dir)
    {
        Vector2 origin = user.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir.normalized,
            data.Range * 2,
            hitLayer
        );

        Debug.DrawRay(origin, dir.normalized * (data.Range * 2), Color.yellow, 1f);

        if (!hit) return;
        
        RaycastHit2D[] explosion = Physics2D.CircleCastAll(hit.transform.position, data.Range / 4, dir.normalized, hitLayer);

        foreach (var a in explosion)
        {
            if (a.transform.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(data.Damage * 2.5f);
                target.ApplyKnockback(dir, 6f, 0.15f);
                target.ApplySlow(SlowPercent, SlowDuration);
            }
        }
        
    }
}
