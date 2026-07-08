using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossProjectile : MonoBehaviour, IDamageable
{
    private float damage;
    private float maxDistance;
    private Vector2 startPosition;
    [SerializeField] private float speed = 8f;
    private bool isInitialized = false;
    private Rigidbody2D rb2d;
    [SerializeField] private int projectileHealth = 1;
    private bool canBeDamaged = true;
    
    private LayerMask targetLayer;
    
    private bool destroyOnHit;
    private string deathReason;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, float range, Vector2 dir, GameObject target, BossType bossType)
    {
        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();

        if (rb2d == null)
        {
            Debug.LogError($"{name} needs a Rigidbody2D to move as a boss projectile.", this);
            Destroy(gameObject);
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        this.damage = damage;
        maxDistance = range;
        startPosition = transform.position;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);
        
        rb2d.linearVelocity = dir.normalized * speed;
        targetLayer = 1<<target.layer;
        canBeDamaged = true;
        destroyOnHit = bossType == BossType.magician;
        deathReason = bossType == BossType.magician
            ? "Hit by Magician Boss projectile"
            : "Hit by Warrior Boss sword wave";
        
        isInitialized = true;
    }

    public void InitEnemyProjectile(float damage, float range, Vector2 dir, GameObject target)
    {
        InitEnemyProjectile(damage, range, dir, target, speed);
    }

    public void InitEnemyProjectile(float damage, float range, Vector2 dir, GameObject target, float projectileSpeed)
    {
        float previousSpeed = speed;
        speed = projectileSpeed;
        Init(damage, range, dir, target, BossType.magician);
        speed = previousSpeed;

        canBeDamaged = false;
        destroyOnHit = true;
        deathReason = "Defeated by ranged enemy projectile";
    }

    private void Update()
    {
        if (!isInitialized) return;

        float traveledDistance = Vector2.Distance(startPosition, transform.position);

        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable target))
        {
            if (1 << collision.gameObject.layer == targetLayer)
            {
                GameResultTracker.Instance?.SetDeathReason(deathReason);
                target.TakeDamage(damage);
                if (destroyOnHit)
                    Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!canBeDamaged)
            return;

        projectileHealth--;
        if (projectileHealth <= 0)
            Destroy(gameObject);
    }

    public void ApplySlow(float percent, float duration) {}

    public void ApplyKnockback(Vector2 dir, float power, float duration) {}
}
