using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour, IPlayerMover
{
    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundBoxSize = new(0.5f, 0.1f);

    [Header("Jump")]
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Movement")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 35f;
    [SerializeField] private float turnDeceleration = 60f;

    [Header("Physics")]
    [SerializeField] private float maxFallSpeed = 25f;

    public bool IsGrounded { get; private set; }

    private Rigidbody2D rb;
    private Vector2 movement;

    private Coroutine knockRoutine;
    private Coroutine slowRoutine;

    private MoveLockType moveLockType = MoveLockType.None;

    private Player player; // ⭐ Player 참조
    private float jumpForce;

    private float coyoteCounter;
    private float jumpBufferCounter;

    private float slowMultiplier = 1f;
    private float defaultGravity;

    private bool isDashing;
    private bool isKnocked;
    private bool jumpHeld;

    private int jumpCount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    public void Init(Player player, float jumpForce)
    {
        this.player = player;
        this.jumpForce = jumpForce;
    }

    public void SetMove(Vector2 move)
    {
        movement = move;
    }

    public void SetJumpPressed()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    public void SetDashPressed(bool dash)
    {
        isDashing = dash;
    }

    private void Update()
    {
        UpdateTimers();
    }

    private void FixedUpdate()
    {
        UpdateGround();
        ApplyFallLimit();
        HandleMove();
        HandleJump();
        HandleJumpCut();
        player?.UpdateMoveAnimation(rb.linearVelocity, movement, IsGrounded);
    }

    private void UpdateGround()
    {
        IsGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundBoxSize,
            0,
            groundLayer
        );

        if (IsGrounded && rb.linearVelocity.y <= 0)
        {
            coyoteCounter = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;

            if (!IsGrounded && jumpCount == 0 && rb.linearVelocity.y < 0)
            {
                jumpCount = 1;
            }
        }
    }

    private void UpdateTimers()
    {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;
    }

    private void HandleJump()
    {
        if (moveLockType == MoveLockType.FullLock || isKnocked)
            return;

        if (jumpBufferCounter <= 0)
            return;

        bool canGroundJump = coyoteCounter > 0;
        bool canAirJump = jumpCount < maxJumpCount;

        if (canGroundJump || canAirJump)
        {
            Jump();
        }
    }

    private void HandleJumpCut()
    {
        if (jumpHeld)
            return;

        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    public void SetJumpHeld(bool held)
    {
        jumpHeld = held;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        jumpBufferCounter = 0;
        coyoteCounter = 0;
        jumpCount++;
    }

    private void HandleMove()
    {
        if (moveLockType == MoveLockType.FullLock || isKnocked)
            return;

        float baseSpeed = player.Stats.MoveSpeed;

        if (isDashing)
            baseSpeed *= 1.3f;

        float maxSpeed = baseSpeed * slowMultiplier;

        float targetSpeed = movement.x * maxSpeed;
        float currentSpeed = rb.linearVelocity.x;

        float accelRate;

        if (moveLockType == MoveLockType.HorizontalOnly)
        {
            targetSpeed = 0;
            accelRate = deceleration;
        }
        else
        {
            accelRate = Mathf.Abs(targetSpeed) > 0.01f
                ? (Mathf.Approximately(Mathf.Sign(targetSpeed), Mathf.Sign(currentSpeed))
                    ? acceleration
                    : turnDeceleration)
                : deceleration;
        }

        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }

    private void ApplyFallLimit()
    {
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -maxFallSpeed
            );
        }
    }

    public void SetMoveLock(MoveLockType type)
    {
        moveLockType = type;

        if (type == MoveLockType.FullLock)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    public void KnockBack(Vector2 dir, float power, float duration)
    {
        if (knockRoutine != null)
            StopCoroutine(knockRoutine);

        knockRoutine = StartCoroutine(KnockBackCor(dir, power, duration));
    }

    public void Slow(float percent, float duration)
    {
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(SlowCor(percent, duration));
    }

    private IEnumerator SlowCor(float percent, float duration)
    {
        slowMultiplier = 1f - percent / 100f;

        yield return new WaitForSeconds(duration);

        slowMultiplier = 1f;
        slowRoutine = null;
    }

    private IEnumerator KnockBackCor(Vector2 dir, float power, float duration)
    {
        isKnocked = true;

        float timer = 0f;

        while (timer < duration)
        {
            rb.linearVelocity = dir * power;
            timer += Time.deltaTime;
            yield return null;
        }

        isKnocked = false;
        knockRoutine = null;
    }
}
