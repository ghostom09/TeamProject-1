using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> onMove;
    public event Action onJump;
    public event Action<bool> onJumpHeld;
    public event Action<bool> onDash;
    public event Action onAttack;



    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed) onMove?.Invoke(context.ReadValue<Vector2>());
        else if(context.canceled) onMove?.Invoke(Vector2.zero);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) onJump?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) onDash?.Invoke(true);
        else if(context.canceled) onDash?.Invoke(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onAttack?.Invoke();
        }
    }
}
