using UnityEngine;

public class SwordIllusionProjectile : MonoBehaviour
{
    private float _damage;
    private float _speed;
    private Transform _target;

    private bool _isFired = false;

    public void Init(float damage)
    {
        _damage = damage;
        _speed = 14f;
    }

    public void Fire(Transform target)
    {
        _target = target;
        _isFired = true;

        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (!_isFired)
            return;
        
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = (_target.position - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        transform.position += (Vector3)(dir * (_speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isFired)
            return;

        if (other.transform == _target)
        {
            if (other.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }
    }
}