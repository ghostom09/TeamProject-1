using System.Collections;
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
        hitLayer = LayerMask.GetMask("Water");
    }

    protected override void Execute(GameObject user, Vector2 dir)
    {
        
        var mover = user.GetComponent<IPlayerMover>();
        
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

        // 3. 데미지 처리
        var damageable = hit.collider.GetComponent<IDamageable>();
        damageable?.TakeDamage(data.Damage * 2.5f);
        damageable?.ApplyKnockback(dir, 6f, 0.15f);

        // 4. 슬로우 처리
        var slowable = hit.collider.GetComponent<IDamageable>();
        slowable?.ApplySlow(SlowPercent, SlowDuration);
        
    }
}
