using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;

    private float damage;
    private Vector2 direction;
    private float maxDistanceSqr;
    private Vector2 startPosition;

    public void Init(Vector2 dir, float dmg, float range)
    {
        direction = dir.normalized;
        damage = dmg;
        maxDistanceSqr = range * range;
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        float distSqr =
            (transform.position - (Vector3)startPosition).sqrMagnitude;

        if (distSqr >= maxDistanceSqr)
        {
            Destroy(gameObject);
        }

    }
    
}