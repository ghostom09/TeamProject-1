using UnityEngine;
using System.Collections;

public class TrackingProjectile : MonoBehaviour, IDamageable
{
    private float damage;
    private float maxDistance;
    private Vector2 startPosition;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float maxLifetime = 12f;
    private bool isInitialized = false;
    
    [SerializeField] private float fireInterval = 5f;
    private float fireTimer = 0f;
    
    private LayerMask targetLayer;
    [SerializeField] private int projectileHealth = 3;

    private Transform targetTransform;
    private float randomOffset;
    private float lifeTimer;
    private bool isFired = false; 

    public void Init(float damage, float range, GameObject target, Vector2 starting)
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        this.damage = damage;
        maxDistance = range;
        targetLayer = 1 << target.layer;
        targetTransform = target.transform;
        startPosition = starting;
        lifeTimer = 0f;
        fireTimer = 0f;
        isFired = false;
        
        isInitialized = true;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
        transform.position = starting;
    }

    private void Fire()
    {
        isFired = true;
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Fire();
        }
        if (!isInitialized) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifetime ||
            (maxDistance > 0f && Vector2.Distance(startPosition, transform.position) >= maxDistance))
        {
            Destroy(gameObject);
            return;
        }
        
        if (!isFired)
        {
            float tilt = Mathf.Sin(Time.time * 2f + randomOffset) * 5f;
            transform.rotation = Quaternion.Euler(0, 0, tilt + 90f); 
        }
        else
        {
            if (targetTransform == null) { Destroy(gameObject); return; }

            Vector2 targetDir = (targetTransform.position - transform.position).normalized;
            
            float rotateSpeed = 10f; 
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            transform.position += transform.up * (speed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        projectileHealth--;
        StartCoroutine(Hit());
        if(projectileHealth <= 0)
            Destroy(gameObject);
    }

    private IEnumerator Hit()
    {
        transform.localScale *= 0.8f;
        damage *= 0.8f;
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFired) return;
    
        if (collision.TryGetComponent(out IDamageable target))
        {
            if (1 << collision.gameObject.layer == targetLayer)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public void ApplySlow(float percent, float duration){}
    public void ApplyKnockback(Vector2 dir, float power, float duration){}
}
