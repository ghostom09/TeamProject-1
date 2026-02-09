using UnityEngine;

public class PlayerInputAttackConnector : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAttack playerAttack;
    public void Connect()
    {
        playerInput.onAttack += playerAttack.Attack;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
}
