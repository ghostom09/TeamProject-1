using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject dummy;

    void UseSKill1()
    {
        Instantiate(dummy, transform.position, Quaternion.identity);
        //skill 사용
    }

    void UseSKill2()
    {
        
    }

    void UseUltra()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            UseSKill1();   
        }
    }
}
