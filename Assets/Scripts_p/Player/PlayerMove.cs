using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMove : MonoBehaviour, IPlayerMover
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private int _maxJumpCount = 2;
    
    public bool isGrounded;
    
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Coroutine _knockRoutine;
    private Coroutine _slowRoutine;
    private MoveLockType _moveLockType = MoveLockType.None;

    private float _originGravity;
    private float _acceleration = 25f;
    private float _deceleration = 35f;
    private float _turnDeceleration = 60f;
    private float _moveSpeed;
    private float _dashSpeed;
    private float _coyoteTime = 0.1f;
    private float _jumpBufferTime = 0.1f;
    private float _groundRadius = 0.2f;
    private float _jumpForce;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private float _slowMultiplier = 1f;
    private float _defaultGravity;  
    private bool _isDashing;
    private bool _isKnocked;
    private bool _isMoveLocked;
    private bool _hasJumpedFromGround;
    private int _jumpCount;
    
    


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _defaultGravity = _rb.gravityScale;
    }

    public void Init(float moveSpeed, float jumpForce)
    {
        this._moveSpeed = moveSpeed;
        this._jumpForce = jumpForce;
        this._dashSpeed = moveSpeed * 1.3f;
    }
    
    public void SetMove(Vector2 move)
    {
        this._movement = move;
    }

    public void SetJumpPressed()
    {
        _jumpBufferCounter = _jumpBufferTime;
    }

    public void SetDashPressed(bool isDashing)
    {
        _isDashing = isDashing;
    }
    private void Update()
    {
        CheckGround();

        if (isGrounded && _rb.linearVelocity.y <= 0)
        {
            _coyoteTimeCounter = _coyoteTime;
            _jumpCount = 0;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0)
            _jumpBufferCounter -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        HandleMove();
        HandleJump();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            _groundRadius,
            groundLayer
        );
    }
    private void HandleJump()
    {
        if (_isMoveLocked)
            return;

        if (_jumpBufferCounter <= 0)
            return;

        // 첫 점프
        if (_jumpCount == 0)
        {
            Jump();

            // 땅에서 점프했는지 기록 
            _hasJumpedFromGround = isGrounded;

            return;
        }

        // 두번째 점프 (땅에서 시작했을 때만)
        if (_hasJumpedFromGround && _jumpCount == 1)
        {
            Jump();
        }
    }
    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);

        _jumpBufferCounter = 0;
        _coyoteTimeCounter = 0;

        _jumpCount++;
    }
    private void HandleMove()
    {
        if (_moveLockType == MoveLockType.FullLock || _isKnocked)
            return;

        if (_moveLockType == MoveLockType.HorizontalOnly)
            return;
        
        float baseSpeed = _isDashing ? _dashSpeed : _moveSpeed;
        float maxSpeed = baseSpeed * _slowMultiplier;
        
        float targetSpeed = _movement.x * maxSpeed;
        float currentSpeed = _rb.linearVelocity.x;

        float accelRate;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            if (Mathf.Sign(targetSpeed) == Mathf.Sign(currentSpeed))
                accelRate = _acceleration;
            else
                accelRate = _turnDeceleration;
        }
        else
        {
            accelRate = _deceleration;
        }

        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        _rb.linearVelocity = new Vector2(newSpeed, _rb.linearVelocity.y);
    }
    
    public void SetMoveLock(MoveLockType type)
    {
        _moveLockType = type;

        if (type == MoveLockType.FullLock)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0f;
        }
        else
        {
            _rb.gravityScale = _defaultGravity;
        }
    }
    public void KnockBack(Vector2 dir, float force, float duration)
    {
        if (_knockRoutine != null)
            StopCoroutine(_knockRoutine);

        _knockRoutine = StartCoroutine(KnockBackCor(dir, force, duration));
    }

    public void Slow(float slowPercent, float slowDuration)
    {
        if (_slowRoutine != null)
            StopCoroutine(_slowRoutine);

        _slowRoutine = StartCoroutine(ApplySlow(slowPercent, slowDuration));
    }

    private IEnumerator ApplySlow(float percent, float duration)
    {
        _slowMultiplier = 1f - (percent / 100f);

        yield return new WaitForSeconds(duration);

        _slowMultiplier = 1f;
        _slowRoutine = null;
    }
    
    private IEnumerator KnockBackCor(Vector2 dir, float power, float duration)
    {
        _isKnocked = true;

        float timer = 0f;

        while (timer < duration)
        {
            _rb.linearVelocity = dir * power;
            timer += Time.deltaTime;
            yield return null;
        }

        _isKnocked = false;
        _knockRoutine = null;
    }
    
    
}
