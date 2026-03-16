using System.Collections;
using UnityEngine;

public class SwordUlt : SkillBase
{
    private const int HitCount = 20;
    private const float HitInterval = 0.1f;

    private LayerMask enemyLayer;

    public SwordUlt()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected override bool Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        // 게이지 부족하면 실행 안 함
        if (!player.UseGauge(80))
            return false;

        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(UltimateRoutine(user));

        return true;
    }

    private IEnumerator UltimateRoutine(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        PlayerMove playerMove = user.GetComponent<PlayerMove>();

        player.isInvincible = true;
        player.isUsingUltimate = true;

        playerMove.SetMoveLock(MoveLockType.FullLock);

        for (int i = 0; i < HitCount; i++)
        {
            DoSlash(user, player);
            yield return new WaitForSeconds(HitInterval);
        }

        yield return new WaitForSeconds(0.8f);

        DoFinalStrike(user, player);

        player.isInvincible = false;
        player.isUsingUltimate = false;

        playerMove.SetMoveLock(MoveLockType.None);
    }

    private void DoSlash(GameObject user, Player player)
    {
        Vector2 origin = user.transform.position;

        float damage = player.Stats.Damage * 0.45f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            data.Range * 2,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(damage);

                enemy.ApplySlow(99, 0.1f);

                Vector2 knockDir =
                    ((Vector2)hit.transform.position - origin).normalized;

                enemy.ApplyKnockback(knockDir, 1f, 0.15f);
            }
        }
    }

    private void DoFinalStrike(GameObject user, Player player)
    {
        Vector2 origin = user.transform.position;

        float finalDamage = player.Stats.Damage * 5.7f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            data.Range * 3f,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(finalDamage);

                Vector2 knockDir =
                    ((Vector2)hit.transform.position - origin).normalized;

                enemy.ApplyKnockback(knockDir, 10f, 0.15f);
            }
        }
    }
}