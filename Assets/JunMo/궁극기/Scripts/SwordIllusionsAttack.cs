using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SwordIllusionsAttack : MonoBehaviour
{
    private const float RetargetRadius = 7f;

    private float _damage;
    private float _speed;
    private Transform _target;
    private LayerMask _enemyLayer;

    private bool _isFired;

    private float _hoverTimer;
    private Transform _playerTransform;
    private Vector3 _relativeOffset;
    
    private TrailRenderer _trail;
    private Coroutine _lifeRoutine;

    private void OnEnable()
    {
        _trail = GetComponentInChildren<TrailRenderer>();
        _enemyLayer = LayerMask.GetMask("Enemy");
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
        _lifeRoutine = StartCoroutine(SwordBoom());
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

        if (!HasValidTarget() && !TryRetarget())
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

            if (_lifeRoutine != null)
                StopCoroutine(_lifeRoutine);

            ObjectPoolManager.Instance.Release(ObjectName.SwordIllusions, gameObject);
        }
    }

    private bool HasValidTarget()
    {
        if (_target == null || !_target.gameObject.activeInHierarchy)
            return false;

        Collider2D[] colliders = _target.GetComponents<Collider2D>();
        if (colliders == null || colliders.Length == 0)
            return true;

        foreach (Collider2D col in colliders)
        {
            if (col != null && col.enabled)
                return true;
        }

        return false;
    }

    private bool TryRetarget(Transform ignoredTarget = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, RetargetRadius, _enemyLayer);

        Transform nextTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null || !hit.gameObject.activeInHierarchy)
                continue;

            Transform candidate = hit.transform;
            if (candidate == ignoredTarget || candidate == _target)
                continue;

            if (!hit.TryGetComponent<IDamageable>(out _) &&
                hit.GetComponentInParent<IDamageable>() == null)
                continue;

            float distance = ((Vector2)candidate.position - (Vector2)transform.position).sqrMagnitude;
            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            nextTarget = candidate;
        }

        _target = nextTarget;
        return _target != null;
    }
}
