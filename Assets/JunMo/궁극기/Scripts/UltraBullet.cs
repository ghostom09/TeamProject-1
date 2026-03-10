using UnityEngine;

public class UltraBullet : MonoBehaviour, IBulletBehavior
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;

    private Vector2 startPosition;
    private float maxDistance;

    public void Initialize(Vector2 direction, float speed, float distance)
    {
        startPosition = transform.position;
        maxDistance = distance;

        rb.linearVelocity = direction * speed;
    }
    
    public void BulletDestroy()
    {
        Destroy(gameObject);
    }
}
