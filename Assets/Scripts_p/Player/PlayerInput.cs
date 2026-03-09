using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> onMove;
    public event Action onJump;
    public event Action<bool> setJumpHeld;
    public event Action<bool> onDash;
    public event Action onAttack;
    public event Action<int> onSkills;



    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed) onMove?.Invoke(context.ReadValue<Vector2>());
        else if(context.canceled) onMove?.Invoke(Vector2.zero);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onJump?.Invoke();
            setJumpHeld?.Invoke(true);
        }
        else if (context.canceled)
        {
            setJumpHeld?.Invoke(false);
        }
        
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

    public void OnSkill_1(InputAction.CallbackContext context)
    {
        if(context.performed) onSkills?.Invoke(0);
    }

    public void OnSkill_2(InputAction.CallbackContext context)
    {
        if(context.performed) onSkills?.Invoke(1);
    }

    public void OnSkill_3(InputAction.CallbackContext context)
    {
        if(context.performed) onSkills?.Invoke(2);
    }
}
