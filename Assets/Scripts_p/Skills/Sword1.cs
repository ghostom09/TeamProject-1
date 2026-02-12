using UnityEngine;

public class Sword1 : SkillBase
{
    private float maxDistance = 6f;
    private LayerMask enemyLayer;
    public Sword1() : base(3f)
    {
        enemyLayer = LayerMask.GetMask("Water");
        Debug.Log("레이어 설정 완료");
    }
    protected override void Execute(GameObject user, Vector2 dir)
    {
        dir = dir.normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(
            user.transform.position,
            dir,
            data.Range * 3,
            enemyLayer
        );

        if (hit.collider == null)
        {
            Debug.Log("신법 실패: 적 없음");
            return;
        }

        // 2. 적에게 순간이동
        Vector2 enemyPos = hit.collider.transform.position;
        user.transform.position = enemyPos;

        // 3. 데미지
        if (hit.collider.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(data.Damage * 1.2f);
            target.ApplyKnockback(dir, 6f, 0.15f);
        }

        // 4. 연출용 로그
        Debug.Log($"신법 성공! 대상: {hit.collider.name}");
        lastUsedTime = Time.time;
    }
}
