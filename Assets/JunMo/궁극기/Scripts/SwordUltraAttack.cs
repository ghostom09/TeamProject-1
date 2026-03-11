using UnityEngine;

public class SwordUltraAttack : MonoBehaviour
{
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