using UnityEngine;

public class SwordIllusionProjectile : MonoBehaviour
{
    private float _damage;
    private float _speed;
    private Transform _target;

    private bool _isFired = false;

    private float _hoverTimer;
    private Transform _playerTransform;
    private Vector3 _relativeOffset; // 생성 시 부여받은 상대적 위치

    private Vector3 _startPos;
    private float _randomOffset;

    public void Init(float damage)
    {
        _damage = damage;
        _speed = 14f;
        _startPos = transform.position;
        _randomOffset = Random.Range(0f, Mathf.PI * 2f);
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
        {
            float hoverY = Mathf.Sin(Time.time * 3f + _randomOffset) * 0.15f;
            
            float shakeX = Mathf.Cos(Time.time * 20f + _randomOffset) * 0.02f;
            
            float tilt = Mathf.Sin(Time.time * 2f + _randomOffset) * 5f;

            transform.position = _startPos + new Vector3(shakeX, hoverY, 0);
            transform.rotation = Quaternion.Euler(0, 0, tilt + 90f); 
            return;
        }
        
        if (_target == null) { Destroy(gameObject); return; }

        Vector2 targetDir = (_target.position - transform.position).normalized;
        
        float rotateSpeed = 10f; 
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        transform.position += transform.up * (_speed * Time.deltaTime);
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