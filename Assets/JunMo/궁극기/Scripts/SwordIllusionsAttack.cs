using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SwordIllusionsAttack : MonoBehaviour
{
   private float _damage;
    private float _speed;
    private Transform _target;

    private bool _isFired;

    private float _hoverTimer;
    private Transform _playerTransform;
    private Vector3 _relativeOffset;
    
    private TrailRenderer _trail;

    private void OnEnable()
    {
        _trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnDisable()
    {
        _trail.Clear();
    }

    public void Init(float damage)
    {
        _trail.enabled = true;
        _damage = damage;
        _speed = 14f;
        _target = null;
        _isFired = false;
    }

    public void Fire(Transform target)
    {
        _target = target;
        _isFired = true;
        StartCoroutine(SwordBoom());
    }

    private IEnumerator SwordBoom()
    {
        yield return new WaitForSeconds(3f);
        ObjectPoolManager.Instance.Release(ObjectName.SwordIllusions, gameObject);
    }

    private void Update()
    {
        if (!_isFired)
        {
            return;
        }

        if (!_target)
        {
            ObjectPoolManager.Instance.Release(ObjectName.SwordIllusions, gameObject);
            return;
        }

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
            StopCoroutine(SwordBoom());
            ObjectPoolManager.Instance.Release(ObjectName.SwordIllusions, gameObject);
        }
    }
}
