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

        for (int i = 0; i < 3; i++)
        {
            Vector3 offset = new Vector3(i - 1, 0.5f, 0); // 좌우 배치

            GameObject obj = Instantiate(
                illusionPrefab,
                transform.position + offset,
                Quaternion.identity
            );

            var illusion = obj.GetComponent<SwordIllusionProjectile>();
            illusion.Init(damage);

            _illusions.Add(illusion);
        }
    }
    
    public void FireIllusions(Transform target)
    {
        StartCoroutine(FireSequential(target));
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
