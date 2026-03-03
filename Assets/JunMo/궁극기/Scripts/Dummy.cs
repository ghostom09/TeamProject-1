using UnityEngine;

public class Dummy : MonoBehaviour
{
    private float lifeTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        
    }
}
