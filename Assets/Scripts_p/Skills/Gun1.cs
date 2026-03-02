using System.Collections;
using Unity.VisualScripting;
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
        hitLayer = LayerMask.GetMask("Enemy","Wall");
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
        var mover = user.GetComponent<IPlayerMover>();
        Player player = user.GetComponent<Player>();

        float waitTime = ignoreMoveLock ? 0f : StopTime;

        if (!ignoreMoveLock)
            mover?.SetMoveLock(true);

        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        mover?.SetMoveLock(false);

        FireHitScan(user, dir, player);
    }

    private void FireHitScan(GameObject user, Vector2 dir, Player player)
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
        
        Collider2D[] explosion = Physics2D.OverlapCircleAll(
            hit.point,
            data.Range / 4,
            hitLayer
        );

        foreach (var col in explosion)
        {
            if (col.TryGetComponent<IDamageable>(out var target))
            {
                
                target.TakeDamage(data.Damage * 2.5f);
                target.ApplyKnockback(dir, 6f, 0.15f);
                target.ApplySlow(SlowPercent, SlowDuration);
                player.AddGauge(1);
            }
        }
        
    }
}
