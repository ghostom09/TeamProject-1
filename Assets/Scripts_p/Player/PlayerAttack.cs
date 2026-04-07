using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject illusionPrefab;
    
    public float attackTime;
    
    private INormalAttack _normal;
    private Player _player;
    private Coroutine _currentAttackCoroutine;
                                  
    private int _illusionCount;
    private float _illusionDamage;
    private Transform _pivot;
    
    private List<SwordIllusionsAttack> _illusions 
        = new();

    public void Init(CharacterData data)
    {
        _normal = data.JobType switch
        {
           jobType.Sword => new SwordNormalAttack(),
           jobType.Gun   => new GunNormalAttack(),
            _ => null
        };
        _normal?.Init(data);
        _player = GetComponent<Player>();
    }

    public void Attack()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        
        _normal.TryAttack(gameObject, dir);
    }
    public INormalAttack GetNormalAttack()
    {
        return _normal;
    }
    
    public void SpawnIllusions(float damage)
    {
        ClearIllusions();
    
        int count = 3;
        float radius = 0.7f;
        float totalAngle = 80f;
    
        _pivot = new GameObject("IllusionPivot").transform;
        _pivot.position = transform.position;
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            float radian = angle * Mathf.Deg2Rad;
    
            Vector3 offset = new Vector3(
                Mathf.Cos(radian),
                Mathf.Sin(radian),
                0f
            ) * radius;
    
            Vector3 spawnPos = transform.position + offset;
            
            GameObject obj = ObjectPoolManager.Instance.Get
                (ObjectName.SwordIllusions, spawnPos, Quaternion.Euler(0, 0, angle - 90f));
    
            var illusion = obj.GetComponent<SwordIllusionsAttack>();
            illusion.Init(damage);
            obj.transform.SetParent(_pivot);
    
            _illusions.Add(illusion);
        }
    }
    
    public void FireIllusions(Transform target, GameObject user)
    {
        Player player = user.GetComponent<Player>();
        StartCoroutine(FireSequential(target, player));
    }

    private IEnumerator FireSequential(Transform target, Player player) // Player 매개변수 추가
    {
        foreach (var illusion in _illusions)
        {
            if (illusion != null)
            {
                // 환영을 발사할 때 player 정보도 같이 넘겨줍니다.
                illusion.Fire(target); 
                yield return new WaitForSeconds(0.1f);
            }
        }

        _illusions.Clear();
    }

    private void ClearIllusions()
    {
        if (_illusions.Count > 0)
        {
            foreach (var illusion in _illusions)
            {
                ObjectPoolManager.Instance.Release(ObjectName.SwordIllusions, illusion.gameObject);
            }
        }
        _illusions.Clear();
    }

    private void Update()
    {
        if (_pivot)
        {
            _pivot.Rotate(0, 0, 30f * Time.deltaTime);
        }
    }
}
