using UnityEngine;
using System.Collections;

public class MagicianShortAttack : IBossSkillStrategy
{
    private float damage;
    private float attackRange;
    
    private Vector2 dir;
    private BossAttack bossAttack;
    
    private Vector2 bulletOffset;
    private const float CastDelay = 0.2f;
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject target)
    {
        damage = data.damage;
        attackRange = data.attackRange;
        this.bossAttack = bossAttack;
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
        boss.GetComponent<MonoBehaviour>().
            StartCoroutine(AttackRoutine(boss, target, onComplete));
    }
    
    private IEnumerator AttackRoutine(GameObject boss, GameObject target, System.Action onComplete)
    {
        yield return new WaitForSeconds(CastDelay);
    
        Vector2 baseDir = (target.transform.position - boss.transform.position).normalized;

        float[] angles = { 0f, 120f, 240f };

        foreach (float angle in angles)
        {
            yield return new WaitForSeconds(0.2f);
            Vector2 shotDir = RotateVector(baseDir, angle);
            bulletOffset = ((Vector2)boss.transform.position + (shotDir * 3));

            bossAttack.SpawnMagicCircle(BossSkillType.shortDistance, bulletOffset, 1.2f, 0.5f);
            bossAttack.ShootTrakingProjectile(shotDir, damage, attackRange, bulletOffset);
        }

        EndAttack(null, onComplete);
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        // 벡터 회전 함수
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
    
        float tx = v.x;
        float ty = v.y;
    
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
