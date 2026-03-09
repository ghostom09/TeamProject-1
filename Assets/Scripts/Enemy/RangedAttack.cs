using System.Collections;
using UnityEngine;

public class RangedAttack : IEnemyAttackStrategy
{
    private float damage;
    private float attackSpeed;
    private float attackRange;
    private float lastAttackTime;
    
    private float distSqr;
    private float interval;

    private const float shootDelay = 0.5f;

    private Rigidbody2D rb2d;
    private EnemyMove enemyMove;

    public void Init(EnemyStats stats, EnemyMove move)
    {
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
        attackRange = stats.attackRange;
        enemyMove =  move;
    }

    public void TryAttack(GameObject self, Transform target, Vector2 direction)
    {
        distSqr = (target.position - self.transform.position).sqrMagnitude;
        interval = 1f / attackSpeed;

        if (Time.time < lastAttackTime + interval || distSqr > attackRange * attackRange)
        {
            return;
        }

        lastAttackTime = Time.time;

        Shoot(self.transform, direction);
    }
    
    private void Shoot(Transform self, Vector2 dir)
    {
        self.GetComponent<MonoBehaviour>().StartCoroutine(ShootCoroutine(self, dir));
    }
    private void DoHitscan(Transform self, Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            self.position,
            dir.normalized,
            attackRange,
            LayerMask.GetMask("Player")
        );

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                target.ApplyKnockback(dir, 3f, 0.15f);
            }
        }
    }
    
    private IEnumerator ShootCoroutine(Transform self, Vector2 dir)
    {
        enemyMove.SetMoveLock(true);
        yield return new WaitForSeconds(shootDelay);
        
        Debug.DrawRay(self.position, dir.normalized * attackRange, Color.cyan, 0.2f);

        DoHitscan(self, dir);
        
        enemyMove.SetMoveLock(false);
    }
}