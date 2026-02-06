using UnityEngine;

public class PlayerInputMoveConnector : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerMove playerMove;

    public void Connect()
    {
        playerInput.onMove += playerMove.SetMove;
        playerInput.onJump += playerMove.SetJumpPressed;
        playerInput.onDash += playerMove.SetDashPressed;
    }

    //테스트
    private void Start()
    {
        Connect();
    }
}
