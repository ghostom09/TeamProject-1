using System.Collections;
using UnityEngine;

public class SwordUlt : SkillBase
{
    private int hitCount = 20;
    private float hitInterval = 0.1f;

    public SwordUlt()
    {
        
    }
    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        // 게이지 부족하면 실행 안 함
        if (!player.UseGauge(80))
            return;
        
        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(UltimateRoutine(user));
    }

    private IEnumerator UltimateRoutine(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        PlayerMove playerMove = user.GetComponent<PlayerMove>();

        player.isInvincible = true;
        player.isUsingUltimate = true;
        playerMove.SetMoveLock(MoveLockType.FullLock);

        
        for (int i = 0; i < hitCount; i++)
        {
            DoSlash(user);
            yield return new WaitForSeconds(hitInterval);
        }
        
        yield return new WaitForSeconds(0.8f);
        
        DoFinalStrike(user);

        

        player.isInvincible = false;
        player.isUsingUltimate = false;
        playerMove.SetMoveLock(MoveLockType.None);
    }
    private void DoSlash(GameObject user)
    {
        float damage = data.Damage * 0.45f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            user.transform.position,
                data.Range * 2,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(damage);
                enemy.ApplySlow(99, 0.1f);

                // 넉백 방향 계산
                Vector2 knockDir = (hit.transform.position - user.transform.position).normalized;
                enemy.ApplyKnockback(knockDir, 1f, 0.15f);
            }
        }
    }
    private void DoFinalStrike(GameObject user)
    {
        float finalDamage = data.Damage * 5.7f; // 강한 한 방

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            user.transform.position,
            data.Range * 3f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(finalDamage);
                
                Vector2 knockDir = (hit.transform.position - user.transform.position).normalized;
                enemy.ApplyKnockback(knockDir, 10f, 0.15f);
            }
        }
    }
}
