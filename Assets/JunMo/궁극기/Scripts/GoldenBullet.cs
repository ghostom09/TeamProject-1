using UnityEngine;

public class GoldenBullet : MonoBehaviour, IBulletBehavior
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private GameObject Boom;

    private Vector2 startPosition;
    private float maxDistance;

    public void Initialize(Vector2 direction, float speed, float distance)
    {
        startPosition = transform.position;
        maxDistance = distance;

        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Instantiate(Boom, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    public void BulletDestroy()
    {
        Instantiate(Boom, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
