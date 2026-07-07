using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;

    private Vector2 startPosition;
    private float maxDistance;
    private bool isReleased;

    public void Initialize(Vector2 direction, float speed, float distance)
    {
        startPosition = transform.position;
        maxDistance = distance;
        isReleased = false;

        trail?.Clear();
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Release();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isReleased)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet") &&
            other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(1f);
            Release();
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Release();
        }
    }

    private void Release()
    {
        if (isReleased)
            return;

        isReleased = true;
        rb.linearVelocity = Vector2.zero;
        ObjectPoolManager.Instance.Release(ObjectName.NormalBullet, gameObject);
    }
}
