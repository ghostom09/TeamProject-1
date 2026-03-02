using UnityEngine;

public class Sword2 : SkillBase
{
    private LayerMask enemyLayer;

    public Sword2()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();
        
        if (player.isUsingUltimate)
            return;
        
        PlayerAttack playerAttack = user.GetComponent<PlayerAttack>();

        dir = dir.normalized;

        float angleRange = 125f;

        Vector2 origin = user.transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            origin,
            3.5f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            Vector2 toTarget = (hit.transform.position - user.transform.position).normalized;

            float angle = Vector2.Angle(dir, toTarget);
            
            if (angle <= angleRange * 0.5f)
            {
                if (hit.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(data.Damage * 1.8f);
                }
            }
        }

        // 환영 생성
        playerAttack.SpawnIllusions(data.Damage * 0.35f);

        lastUsedTime = Time.time;

    }
}