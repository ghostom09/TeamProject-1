using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private INormalAttack _normal;

    public void Init(jobType type)
    {
        _normal = type switch
        {
           jobType.Sword => new SwordNormalAttack(),
           jobType.Gun   => new GunNormalAttack(),
            _ => null
        };
    }

    public void Attack()
    {
        RealAttack(Vector2.zero);
    }
    public void RealAttack(Vector2 dir)
    {
        _normal?.TryAttack(gameObject, dir);
    }
}
