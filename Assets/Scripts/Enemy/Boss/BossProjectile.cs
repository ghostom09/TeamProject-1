using System;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private float damage;
    private float maxDistance;
    private Vector2 startPosition;
    [SerializeField] private float speed = 8f;
    private bool isInitialized = false;
    private Rigidbody2D rb2d;
    
    private LayerMask targetLayer;
    
    private BossType bossType;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, float range, Vector2 dir, GameObject target, BossType bossType)
    {
        this.damage = damage;
        maxDistance = range;
        startPosition = transform.position;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);
        
        rb2d.linearVelocity = dir.normalized * speed;
        targetLayer = 1<<target.layer;
        this.bossType = bossType;
        
        isInitialized = true;
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
                target.TakeDamage(damage);
                if(bossType == BossType.magician)
                    Destroy(gameObject);
            }
        }
    }
}