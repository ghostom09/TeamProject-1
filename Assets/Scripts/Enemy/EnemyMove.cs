using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour, IEnemyMover, IDamageable, IEnemyReset
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int IsMoveLockedHash = Animator.StringToHash("IsMoveLocked");

    [SerializeField] private GameObject movingTarget;
    [SerializeField] private Animator animator;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    [SerializeField] private Transform leftSideGroundCheck;
    [SerializeField] private Transform rightSideGroundCheck;
    
    private Rigidbody2D rb2d;
    private float speed;
    private float attackRange;
    private EnemyType enemyType;
    [SerializeField] private float jumpForce;
    
    [SerializeField] private float groundRadius = 0.35f;
    [SerializeField] private float sideRadius = 0.1f;
    private bool isGrounded;
    private bool isleftWall;
    private bool isrightWall;

    private bool leftSide;
    private bool rightSide;
    private bool isSide;
    private bool isJumping = false;
    
    private float movingInterval;
    [SerializeField] private float jumpPersent = 0.02f;
    [SerializeField] private float reverseDeceleration;

    private float distance;
    private float direction;
    private float deltaX;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private float distanceThreshold = 0.1f;
    
    private bool _isMoveLocked;
    
    private Coroutine knockRoutine;

    private EnemyHit _enemyHit;
    private SpriteRenderer spriteRenderer;
    private bool hasSpeedParam;
    private bool hasWalkParam;
    private bool hasMoveXParam;
    private bool hasYVelocityParam;
    private bool hasIsMovingParam;
    private bool hasIsGroundedParam;
    private bool hasIsJumpingParam;
    private bool hasIsMoveLockedParam;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _enemyHit = GetComponent<EnemyHit>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CacheAnimatorParameters();
    }

    public void SetAnimator(Animator targetAnimator)
    {
        animator = targetAnimator;
        CacheAnimatorParameters();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CacheAnimatorParameters();

        speed = stats.speed;
        jumpForce = stats.jumpForce;
        enemyType = stats.enemyType;
        attackRange = stats.attackRange;
        movingInterval = attackRange * Random.Range(0.7f, 0.9f);
        
        movingTarget = target;
        ResetMovementState();
        SetMoveLock(false);
    }
    
    private void FixedUpdate()
    {
        if(movingTarget == null)
        {
            UpdateAnimator();
            return;
        }
        
        CheckGround();
        CheckWall();
        CheckSide();
        
        if(_isMoveLocked)
        {
            UpdateAnimator();
            return;
        }
        
        horizontalmove();
        
        if (isGrounded)
        {
            isJumping = false;
            if (isleftWall || isrightWall ||
                (movingTarget.transform.position.y > transform.position.y && isSide))
            {
                verticalmove();
            }
            else
            {
                if (movingTarget.transform.position.y > transform.position.y)
                {
                    if(Random.value <= jumpPersent)
                        verticalmove();
                }
            }
        }

        UpdateFacing();
        UpdateAnimator();
    }
    
    private void CheckWall()
    {
        isleftWall = Physics2D.OverlapCircle(
            leftWallCheck.position,
            groundRadius,
            wallLayer
        );
        isrightWall = Physics2D.OverlapCircle(
            rightWallCheck.position,
            groundRadius,
            wallLayer
        );
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            wallLayer
        );
    }

    private void CheckSide()
    {
         leftSide = Physics2D.OverlapCircle(leftSideGroundCheck.position, sideRadius, wallLayer);
         rightSide = Physics2D.OverlapCircle(rightSideGroundCheck.position, sideRadius, wallLayer);
        
        isSide = leftSide ^ rightSide;
    }
    
    private void verticalmove()
    {
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        isJumping = true;
    }

    private void horizontalmove()
    {
        distance = ((movingTarget.transform.position.x - transform.position.x) *
                    (movingTarget.transform.position.x - transform.position.x)) +
                   ((movingTarget.transform.position.y - transform.position.y) *
                    (movingTarget.transform.position.y - transform.position.y));

        direction = movingTarget.transform.position.x - transform.position.x;

        if (direction > -stopThreshold && direction < stopThreshold)
        {
            rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
            return;
        }

        direction = direction < 0f ? -1f : 1f;
        
        if (distance > (movingInterval * movingInterval) - distanceThreshold &&
            distance < (movingInterval * movingInterval) + distanceThreshold)
        {
            rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
        }
        else if (distance > (movingInterval * movingInterval))
        {
            rb2d.linearVelocity = new Vector2(direction * speed, rb2d.linearVelocity.y);
        }
        else
        {
            rb2d.linearVelocity = new Vector2(-direction * speed * reverseDeceleration, rb2d.linearVelocity.y);
        }
    }
    
    public void SetMoveLock(bool value)
    {
        _isMoveLocked = value;
        
        if (value)
        {
            rb2d.linearVelocity = new Vector2(0, 0);
        }

        UpdateAnimator();
    }

    private void ResetMovementState()
    {
        if (knockRoutine != null)
        {
            StopCoroutine(knockRoutine);
            knockRoutine = null;
        }

        isJumping = false;
        isleftWall = false;
        isrightWall = false;
        isSide = false;
        leftSide = false;
        rightSide = false;

        if (rb2d != null)
            rb2d.linearVelocity = Vector2.zero;
    }
    
    public void ApplyKnockback(Vector2 dir, float power, float duration)
    {
        if(!gameObject.activeInHierarchy)
            return;
        if (knockRoutine != null)
            StopCoroutine(knockRoutine);

        knockRoutine = StartCoroutine(Knockback(dir, power, duration));
    }

    private IEnumerator Knockback(Vector2 dir, float power, float duration)
    {
        SetMoveLock(true);
        isJumping = true;
        
        float xDir = Mathf.Sign(dir.x);

        rb2d.linearVelocity = new Vector2(
            xDir * power,
            6f
        );

        yield return new WaitForSeconds(duration);

        SetMoveLock(false);
        knockRoutine = null;
    }

    public void TakeDamage(float damage){_enemyHit.TakeDamage(damage);}

    public void ApplySlow(float percent, float duration){_enemyHit.ApplySlow(percent, duration);}

    private void UpdateFacing()
    {
        if (spriteRenderer == null || Mathf.Abs(rb2d.linearVelocity.x) <= 0.01f)
            return;

        spriteRenderer.flipX = rb2d.linearVelocity.x < 0f;
    }

    private void UpdateAnimator()
    {
        if (animator == null || rb2d == null)
            return;

        float horizontalSpeed = Mathf.Abs(rb2d.linearVelocity.x);

        if (hasSpeedParam)
            animator.SetFloat(SpeedHash, rb2d.linearVelocity.magnitude);
        if (hasWalkParam)
            animator.SetBool(WalkHash, horizontalSpeed > 0.01f && !_isMoveLocked);
        if (hasMoveXParam)
            animator.SetFloat(MoveXHash, rb2d.linearVelocity.x);
        if (hasYVelocityParam)
            animator.SetFloat(YVelocityHash, rb2d.linearVelocity.y);
        if (hasIsMovingParam)
            animator.SetBool(IsMovingHash, horizontalSpeed > 0.01f && !_isMoveLocked);
        if (hasIsGroundedParam)
            animator.SetBool(IsGroundedHash, isGrounded);
        if (hasIsJumpingParam)
            animator.SetBool(IsJumpingHash, isJumping || rb2d.linearVelocity.y > 0.01f);
        if (hasIsMoveLockedParam)
            animator.SetBool(IsMoveLockedHash, _isMoveLocked);
    }

    private void CacheAnimatorParameters()
    {
        hasSpeedParam = false;
        hasWalkParam = false;
        hasMoveXParam = false;
        hasYVelocityParam = false;
        hasIsMovingParam = false;
        hasIsGroundedParam = false;
        hasIsJumpingParam = false;
        hasIsMoveLockedParam = false;

        if (animator == null)
            return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == SpeedHash)
                hasSpeedParam = true;
            else if (parameter.nameHash == WalkHash)
                hasWalkParam = true;
            else if (parameter.nameHash == MoveXHash)
                hasMoveXParam = true;
            else if (parameter.nameHash == YVelocityHash)
                hasYVelocityParam = true;
            else if (parameter.nameHash == IsMovingHash)
                hasIsMovingParam = true;
            else if (parameter.nameHash == IsGroundedHash)
                hasIsGroundedParam = true;
            else if (parameter.nameHash == IsJumpingHash)
                hasIsJumpingParam = true;
            else if (parameter.nameHash == IsMoveLockedHash)
                hasIsMoveLockedParam = true;
        }
    }
}
