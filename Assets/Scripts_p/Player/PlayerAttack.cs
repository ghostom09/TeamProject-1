using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private float attackTime;
    
    private INormalAttack _normal;

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
}
