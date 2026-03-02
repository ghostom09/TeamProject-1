using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;

    private Vector2 startPosition;
    private float maxDistance;

    public void Initialize(Vector2 direction, float speed, float distance, bool ultra)
    {
        startPosition = transform.position;
        maxDistance = distance;

        rb.linearVelocity = direction * speed;

        ApplyTrail(ultra);
    }

    void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void ApplyTrail(bool ultra)
    {
        trail.Clear();

        if (ultra)
            trail.time = 1f; //밋밋하다 싶으면 소닉붐 애니메이션 만들어면 괜찮을듯
        else
            trail.time = 0.04f;
    }
}