using UnityEngine;

public interface IBulletBehavior
{
    public void Initialize(Vector2 direction, float speed, float distance);
    public void BulletDestroy();
}
