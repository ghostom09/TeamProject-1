using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    private Rigidbody2D _rb;
    private Vector2 _movement;
    
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
    private bool _isGrounded;
    private bool _isDashing;
    [SerializeField] private int _jumpCount;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Init(5,13);
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

        if (_isGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
            _jumpCount = 0;
        }
        else
            _coyoteTimeCounter -= Time.deltaTime;

        // Jump Buffer
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
        _isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            _groundRadius,
            groundLayer
        );
    }
    private void HandleJump()
    {
        if (_jumpBufferCounter > 0 && CanJump())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);

            _jumpBufferCounter = 0;
            _coyoteTimeCounter = 0;
            _jumpCount++;
        }
    }
    private bool CanJump()
    {
        if (_coyoteTimeCounter > 0)
            return true;
        
        if (_jumpCount < 2)
            return true;

        return false;
    }

    private void HandleMove()
    {
        float maxSpeed = _isDashing ? _dashSpeed : _moveSpeed;
        
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
}
