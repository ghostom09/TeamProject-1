using UnityEngine;

public class UltraBullet : MonoBehaviour
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

    private void Release()
    {
        if (isReleased)
            return;

        isReleased = true;
        rb.linearVelocity = Vector2.zero;
        ObjectPoolManager.Instance.Release(ObjectName.UltraBullet, gameObject);
    }
}
