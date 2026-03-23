using UnityEngine;
using System;
using System.Collections;

public class InputFilter : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
 
    public event Action<Vector2> onMove;
    public event Action onJump;
    public event Action<bool> setJumpHeld;
    public event Action<bool> onDash;
    public event Action onAttack;
    public event Action<int> onSkills;
 
    private bool isActive;
    
    // 기존에 실행 중인 코루틴을 저장 (중첩 호출 방지용)
    private Coroutine shuffleCoroutine;

    void Awake()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        
        playerInput.onMove      += HandleMove;
        playerInput.onJump      += HandleJump;
        playerInput.setJumpHeld += HandleJumpHeld;
        playerInput.onDash      += HandleDash;
        playerInput.onAttack    += HandleAttack;
        playerInput.onSkills    += HandleSkills;
    }

    private void OnDestroy() // 이벤트 해제
    {
        if (playerInput != null)
        {
            playerInput.onMove      -= HandleMove;
            playerInput.onJump      -= HandleJump;
            playerInput.setJumpHeld -= HandleJumpHeld;
            playerInput.onDash      -= HandleDash;
            playerInput.onAttack    -= HandleAttack;
            playerInput.onSkills    -= HandleSkills;
        }
    }
 
    public void ActivateSkill(float time)
    {
        if (shuffleCoroutine != null) StopCoroutine(shuffleCoroutine);
        shuffleCoroutine = StartCoroutine(ShuffleRoutine(time));
    }

    private IEnumerator ShuffleRoutine(float time)
    {
        isActive = true;
        yield return new WaitForSeconds(time);
        isActive = false;
        shuffleCoroutine = null;
    }
 
    private void HandleMove(Vector2 v)
    {
        onMove?.Invoke(isActive ? new Vector2(-v.x, v.y) : v);
    }

    private void HandleJump()
    {
        if (isActive) onDash?.Invoke(true);
        else onJump?.Invoke();
    }
 
    private void HandleJumpHeld(bool held)
    {
        if (isActive) onDash?.Invoke(held);
        else setJumpHeld?.Invoke(held);
    }
 
    private void HandleDash(bool dash)
    {
        if (isActive)
        {
            if (dash) onJump?.Invoke();
            setJumpHeld?.Invoke(dash);
        }
        else
        {
            onDash?.Invoke(dash);
        }
    }
 
    private void HandleAttack()
    {
        if (isActive) onSkills?.Invoke(1);
        else onAttack?.Invoke();
    }
 
    private void HandleSkills(int index)
    {
        if (!isActive)
        {
            onSkills?.Invoke(index);
            return;
        }
 
        switch (index)
        {
            case 0: onSkills?.Invoke(2); break;
            case 1: onAttack?.Invoke();  break;
            case 2: onSkills?.Invoke(0); break;
        }
    }
}