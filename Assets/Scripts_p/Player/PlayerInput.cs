using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private enum ShuffleKey
    {
        Skill1,
        Skill2,
        Skill3,
        MoveLeft,
        MoveRight,
        Jump,
        Attack
    }

    public event Action<Vector2> onMove;
    public event Action onJump;
    public event Action<bool> setJumpHeld;
    public event Action<bool> onDash;
    public event Action onAttack;
    public event Action<int> onSkills;

    private bool isInputShuffled;
    private bool isInputLocked;
    private Coroutine inputShuffleRoutine;
    private readonly ShuffleKey[] shuffledActions = new ShuffleKey[7];
    private bool physicalLeftHeld;
    private bool physicalRightHeld;
    private bool shuffledLeftHeld;
    private bool shuffledRightHeld;
    private bool shuffledJumpHeld;
    private bool jumpHeld;

    public void ActivateInputShuffle(float duration)
    {
        if (inputShuffleRoutine != null)
        {
            StopCoroutine(inputShuffleRoutine);
            ResetShuffledInputState();
        }

        ShuffleInputMap();
        UIManager.Instance?.ShowMagicianUltimateEffect(duration);
        inputShuffleRoutine = StartCoroutine(InputShuffleRoutine(duration));
    }

    public void DeactivateInputShuffle()
    {
        if (inputShuffleRoutine != null)
        {
            StopCoroutine(inputShuffleRoutine);
            inputShuffleRoutine = null;
        }

        if (!isInputShuffled)
            return;

        isInputShuffled = false;
        ResetShuffledInputState();
        UIManager.Instance?.HideMagicianUltimateEffect();
    }

    public void SetInputLocked(bool locked)
    {
        isInputLocked = locked;

        if (locked)
        {
            ResetShuffledInputState();
            jumpHeld = false;
            onMove?.Invoke(Vector2.zero);
            onDash?.Invoke(false);
            setJumpHeld?.Invoke(false);
        }
    }

    private IEnumerator InputShuffleRoutine(float duration)
    {
        isInputShuffled = true;
        yield return new WaitForSeconds(duration);
        isInputShuffled = false;
        ResetShuffledInputState();
        inputShuffleRoutine = null;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (isInputShuffled)
        {
            Vector2 move = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
            UpdatePhysicalMoveKeys(move.x);
            return;
        }

        if(context.performed)
        {
            Vector2 move = context.ReadValue<Vector2>();
            onMove?.Invoke(move);
        }
        else if(context.canceled) onMove?.Invoke(Vector2.zero);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (isInputShuffled)
        {
            HandlePhysicalKey(ShuffleKey.Jump, context.performed, context.canceled);
            return;
        }

        if ((context.started || context.performed) && !jumpHeld)
        {
            jumpHeld = true;
            onJump?.Invoke();
            setJumpHeld?.Invoke(true);
        }
        else if (context.canceled)
        {
            jumpHeld = false;
            setJumpHeld?.Invoke(false);
        }
        
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (context.performed) onDash?.Invoke(true);
        else if(context.canceled) onDash?.Invoke(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (isInputShuffled)
        {
            HandlePhysicalKey(ShuffleKey.Attack, context.started, context.canceled);
            return;
        }

        if (context.started)
        {
            onAttack?.Invoke();
        }
    }

    public void OnSkill_1(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (isInputShuffled)
        {
            HandlePhysicalKey(ShuffleKey.Skill1, context.performed, context.canceled);
            return;
        }

        if(context.performed) onSkills?.Invoke(0);
    }

    public void OnSkill_2(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (isInputShuffled)
        {
            HandlePhysicalKey(ShuffleKey.Skill2, context.performed, context.canceled);
            return;
        }

        if(context.performed) onSkills?.Invoke(1);
    }

    public void OnSkill_3(InputAction.CallbackContext context)
    {
        if (isInputLocked) return;

        if (isInputShuffled)
        {
            HandlePhysicalKey(ShuffleKey.Skill3, context.performed, context.canceled);
            return;
        }

        if(context.performed) onSkills?.Invoke(2);
    }

    private void UpdatePhysicalMoveKeys(float horizontal)
    {
        bool leftHeld = horizontal < -0.5f;
        bool rightHeld = horizontal > 0.5f;

        if (leftHeld != physicalLeftHeld)
        {
            physicalLeftHeld = leftHeld;
            DispatchShuffledKey(ShuffleKey.MoveLeft, leftHeld);
        }

        if (rightHeld != physicalRightHeld)
        {
            physicalRightHeld = rightHeld;
            DispatchShuffledKey(ShuffleKey.MoveRight, rightHeld);
        }
    }

    private void HandlePhysicalKey(ShuffleKey key, bool pressed, bool released)
    {
        if (pressed)
            DispatchShuffledKey(key, true);

        if (released)
            DispatchShuffledKey(key, false);
    }

    private void DispatchShuffledKey(ShuffleKey physicalKey, bool pressed)
    {
        ShuffleKey action = shuffledActions[(int)physicalKey];

        if (pressed)
            PressShuffledAction(action);
        else
            ReleaseShuffledAction(action);
    }

    private void PressShuffledAction(ShuffleKey action)
    {
        switch (action)
        {
            case ShuffleKey.Skill1:
                onSkills?.Invoke(0);
                break;
            case ShuffleKey.Skill2:
                onSkills?.Invoke(1);
                break;
            case ShuffleKey.Skill3:
                onSkills?.Invoke(2);
                break;
            case ShuffleKey.MoveLeft:
                shuffledLeftHeld = true;
                UpdateShuffledMove();
                break;
            case ShuffleKey.MoveRight:
                shuffledRightHeld = true;
                UpdateShuffledMove();
                break;
            case ShuffleKey.Jump:
                if (shuffledJumpHeld)
                    return;

                shuffledJumpHeld = true;
                onJump?.Invoke();
                setJumpHeld?.Invoke(true);
                break;
            case ShuffleKey.Attack:
                onAttack?.Invoke();
                break;
        }
    }

    private void ReleaseShuffledAction(ShuffleKey action)
    {
        switch (action)
        {
            case ShuffleKey.MoveLeft:
                shuffledLeftHeld = false;
                UpdateShuffledMove();
                break;
            case ShuffleKey.MoveRight:
                shuffledRightHeld = false;
                UpdateShuffledMove();
                break;
            case ShuffleKey.Jump:
                if (!shuffledJumpHeld)
                    return;

                shuffledJumpHeld = false;
                setJumpHeld?.Invoke(false);
                break;
        }
    }

    private void UpdateShuffledMove()
    {
        float x = 0f;

        if (shuffledLeftHeld && !shuffledRightHeld)
            x = -1f;
        else if (shuffledRightHeld && !shuffledLeftHeld)
            x = 1f;

        onMove?.Invoke(new Vector2(x, 0f));
    }

    private void ShuffleInputMap()
    {
        ShuffleKey[] values =
        {
            ShuffleKey.Skill1,
            ShuffleKey.Skill2,
            ShuffleKey.Skill3,
            ShuffleKey.MoveLeft,
            ShuffleKey.MoveRight,
            ShuffleKey.Jump,
            ShuffleKey.Attack
        };

        for (int attempt = 0; attempt < 32; attempt++)
        {
            for (int i = 0; i < values.Length; i++)
                shuffledActions[i] = values[i];

            for (int i = 0; i < shuffledActions.Length; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, shuffledActions.Length);
                ShuffleKey temp = shuffledActions[i];
                shuffledActions[i] = shuffledActions[randomIndex];
                shuffledActions[randomIndex] = temp;
            }

            if (!HasSameKeyMapping())
                return;
        }

        for (int i = 0; i < values.Length; i++)
            shuffledActions[i] = values[(i + 1) % values.Length];
    }

    private bool HasSameKeyMapping()
    {
        for (int i = 0; i < shuffledActions.Length; i++)
        {
            if ((int)shuffledActions[i] == i)
                return true;
        }

        return false;
    }

    private void ResetShuffledInputState()
    {
        physicalLeftHeld = false;
        physicalRightHeld = false;

        if (shuffledLeftHeld || shuffledRightHeld)
        {
            shuffledLeftHeld = false;
            shuffledRightHeld = false;
            onMove?.Invoke(Vector2.zero);
        }

        if (shuffledJumpHeld)
        {
            shuffledJumpHeld = false;
            setJumpHeld?.Invoke(false);
        }

        jumpHeld = false;
    }
}
