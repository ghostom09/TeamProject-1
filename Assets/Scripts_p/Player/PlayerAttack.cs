using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject illusionPrefab;
    
    public float attackTime;
    
    private INormalAttack _normal;
    private Player _player;
    
                                  
    private int _illusionCount;
    private float _illusionDamage;
    
    private List<SwordIllusionProjectile> _illusions 
        = new List<SwordIllusionProjectile>();

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
        hitBox?.SetActive(false);
    }

    public void Attack()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;

        if (_normal.TryAttack(gameObject, dir, hitBox))
        {
            StartCoroutine(AttackCoroutine(dir));
        }
    }
    private IEnumerator AttackCoroutine(Vector2 dir)
    {
        yield return new WaitForSeconds(attackTime);
        _normal.EndAttack(gameObject, hitBox);
    }
    
    public INormalAttack GetNormalAttack()
    {
        return _normal;
    }
    
    public void SpawnIllusions(float damage)
    {
        ClearIllusions();
    
        int count = 3;
        float radius = 1.5f;
        float totalAngle = 80f;
    
        for (int i = 0; i < count; i++)
        {
            float angle = (i - (count - 1) / 2f) * (totalAngle / (count - 1));
            
            float radian = (angle + 90f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * radius;
            
            Vector3 spawnPos = transform.position + offset;
            
            Quaternion spawnRot = Quaternion.Euler(0, 0, angle);
    
            GameObject obj = Instantiate(illusionPrefab, spawnPos, spawnRot);
    
            var illusion = obj.GetComponent<SwordIllusionProjectile>();
            illusion.Init(damage); 
    
            _illusions.Add(illusion);
        }
    }
    
    public void FireIllusions(Transform target, GameObject user)
    {
        StartCoroutine(FireSequential(target));
        user.GetComponent<Player>().AddGauge(3);
    }

    private IEnumerator FireSequential(Transform target)
    {
        foreach (var illusion in _illusions)
        {
            if (illusion != null)
            {
                illusion.Fire(target);
                yield return new WaitForSeconds(0.1f);
            }
        }

        _illusions.Clear();
    }

    public void ClearIllusions()
    {
        foreach (var illusion in _illusions)
        {
            if (illusion != null)
                Destroy(illusion.gameObject);
        }

        _illusions.Clear();
    }
}
