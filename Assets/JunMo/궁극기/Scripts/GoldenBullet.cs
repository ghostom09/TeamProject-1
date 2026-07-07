using UnityEngine;

public class GoldenBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private GameObject Boom;

    private Vector2 startPosition;
    private float maxDistance;
    private Vector2 dir;
    private bool isReleased;
    private bool explodeAtMaxDistance;
    private float boomScaleDistance;

    public void Initialize(Vector2 direction, float speed, float distance)
    {
        Initialize(direction, speed, distance, false);
    }

    public void Initialize(Vector2 direction, float speed, float distance, bool explodeAtMaxDistance)
    {
        Initialize(direction, speed, distance, explodeAtMaxDistance, distance);
    }

    public void Initialize(Vector2 direction, float speed, float distance, bool explodeAtMaxDistance, float boomScaleDistance)
    {
        startPosition = transform.position;
        maxDistance = distance;
        dir = direction;
        isReleased = false;
        this.explodeAtMaxDistance = explodeAtMaxDistance;
        this.boomScaleDistance = boomScaleDistance;

        trail?.Clear();
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            transform.position = startPosition + dir.normalized * maxDistance;

            if (explodeAtMaxDistance)
                SpawnBoom();

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
            SpawnBoom();
            Release();
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            SpawnBoom();
            Release();
        }
    }

    private void SpawnBoom()
    {
        GameObject obj = ObjectPoolManager.Instance.Get
            (ObjectName.Boom, (Vector2)transform.position - dir, Quaternion.identity);
        obj.transform.localScale = new Vector2(boomScaleDistance / 4, boomScaleDistance / 4);
    }

    private void Release()
    {
        if (isReleased)
            return;

        isReleased = true;
        rb.linearVelocity = Vector2.zero;
        ObjectPoolManager.Instance.Release(ObjectName.GoldenBullet, gameObject);
    }
}
