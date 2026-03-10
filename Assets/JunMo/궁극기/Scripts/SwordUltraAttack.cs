using UnityEngine;

public class SwordUltraAttack : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail1;
    [SerializeField] private TrailRenderer trail2;

    Vector2 target;
    float speed;

    public void Initialize(Vector2 targetPos, float s)
    {
        target = targetPos;
        speed = s;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }
}