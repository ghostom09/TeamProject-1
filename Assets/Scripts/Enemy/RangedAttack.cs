using System.Collections;
using UnityEngine;

public class RangedAttack : IEnemyAttackStrategy
{
    private const float ProjectileSpawnOffset = 0.6f;

    private float damage;
    private float attackSpeed;
    private float attackRange;
    private float lastAttackTime;
    
    private float distSqr;
    private float interval;

    private const float shootDelay = 0.5f;

    private Rigidbody2D rb2d;
    private EnemyMove enemyMove;
    private GameObject projectilePrefab;

    public RangedAttack(GameObject projectilePrefab)
    {
        this.projectilePrefab = projectilePrefab;
    }

    public void Init(EnemyStats stats, EnemyMove move)
    {
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
        attackRange = stats.attackRange;
        enemyMove =  move;
    }

    public bool TryAttack(GameObject self, Transform target, Vector2 direction)
    {
        distSqr = (target.position - self.transform.position).sqrMagnitude;
        interval = 1f / attackSpeed;

        if (Time.time < lastAttackTime + interval || distSqr > attackRange * attackRange)
        {
            return false;
        }

        lastAttackTime = Time.time;

        Shoot(self.transform, target, direction);
        return true;
    }
    
    private void Shoot(Transform self, Transform target, Vector2 dir)
    {
        self.GetComponent<MonoBehaviour>().StartCoroutine(ShootCoroutine(self, target, dir));
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
                GameResultTracker.Instance?.SetDeathReason("Defeated by ranged enemy");
                target.TakeDamage(damage);
                target.ApplyKnockback(dir, 3f, 0.15f);
            }
        }
    }
    
    private void SpawnProjectile(Transform self, Transform target, Vector2 dir)
    {
        if (projectilePrefab == null || target == null)
        {
            DoHitscan(self, dir);
            return;
        }

        Vector2 shotDir = dir.normalized;
        Vector2 spawnPosition = (Vector2)self.position + shotDir * ProjectileSpawnOffset;
        GameObject obj = Object.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        BossProjectile projectile = obj.GetComponent<BossProjectile>();
        if (projectile == null)
        {
            Object.Destroy(obj);
            DoHitscan(self, dir);
            return;
        }

        projectile.InitEnemyProjectile(damage, attackRange, shotDir, target.gameObject);
    }

    private IEnumerator ShootCoroutine(Transform self, Transform target, Vector2 dir)
    {
        enemyMove.SetMoveLock(true);
        yield return new WaitForSeconds(shootDelay);
        
        Debug.DrawRay(self.position, dir.normalized * attackRange, Color.cyan, 0.2f);

        SpawnProjectile(self, target, dir);
        
        enemyMove.SetMoveLock(false);
    }
}
