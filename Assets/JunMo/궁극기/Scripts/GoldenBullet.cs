using UnityEngine;

public class GoldenBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private GameObject Boom;

    private Vector2 startPosition;
    private float maxDistance;
    private Vector2 dir;

    public void Initialize(Vector2 direction, float speed, float distance)
    {
        startPosition = transform.position;
        maxDistance = distance;
        dir = direction;

        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            ObjectPoolManager.Instance.Release(ObjectName.GoldenBullet, gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            GameObject obj = ObjectPoolManager.Instance.Get
                (ObjectName.Boom, (Vector2)transform.position - dir, Quaternion.identity);
            obj.transform.localScale = new Vector2(maxDistance / 4, maxDistance / 4);
            ObjectPoolManager.Instance.Release(ObjectName.GoldenBullet, gameObject);
        }
    }
}
