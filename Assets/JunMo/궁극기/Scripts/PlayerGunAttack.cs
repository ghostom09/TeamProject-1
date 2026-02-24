using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunAttack : MonoBehaviour
{
    [SerializeField] private GunNormalAttack gun;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject user;
    
    public void OnEnable()
    {
        playerInput.onAttack += Attack;
    }

    void Attack()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        Debug.DrawRay(user.transform.position, dir.normalized * 12f, Color.red, 0.5f);
    }
}
