using UnityEngine;

public class Boom : MonoBehaviour
{
    private float lifeTime = 0.2f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
