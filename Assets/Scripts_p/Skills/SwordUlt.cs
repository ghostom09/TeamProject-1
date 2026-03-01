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
        playerMove.SetMoveLock(true);
        
        
        for (int i = 0; i < hitCount; i++)
        {
            DoSlash(user);
            yield return new WaitForSeconds(hitInterval);
        }

        player.isInvincible = false;
        player.isUsingUltimate = false;
        playerMove.SetMoveLock(false);
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
            }
        }
    }
}
