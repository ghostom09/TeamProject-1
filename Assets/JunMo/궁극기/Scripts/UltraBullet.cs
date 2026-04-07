using UnityEngine;

public class UltraBullet : MonoBehaviour
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
    
    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            ObjectPoolManager.Instance.Release(ObjectName.UltraBullet, gameObject);
        }
    }
}
