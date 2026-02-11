using UnityEngine;

public class Test : MonoBehaviour, IDamageable
{

    public float hp = 99999999;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
    }
}
