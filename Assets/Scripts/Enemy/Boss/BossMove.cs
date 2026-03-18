using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BossMove : MonoBehaviour, IBossReset, IEnemyMover, IDamageable
{
    [SerializeField] private GameObject movingTarget;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    [SerializeField] private Transform leftSideGroundCheck;
    [SerializeField] private Transform rightSideGroundCheck;
    
    private Rigidbody2D rb2d;
    private float speed;
    private float attackRange;
    private BossType bossType;
    [SerializeField] private float jumpForce;
    
    [SerializeField] private float groundRadius = 0.35f;
    [SerializeField] private float sideRadius = 0.1f;
    public bool isGrounded;
    private bool isleftWall;
    private bool isrightWall;

    private bool leftSide;
    private bool rightSide;
    private bool isSide;
    private bool isJumping = false;
    
    private float interval;
    [SerializeField] private float jumpPersent = 0.02f;
    [SerializeField] private float reverseDeceleration;

    private float distance;
    private float direction;
    private float deltaX;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private float distanceThreshold = 0.1f;
    
    private bool _isMoveLocked;
    public float lookSide;
    
    private Coroutine knockRoutine;

    private BossHit _bossHit;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _bossHit = GetComponent<BossHit>();
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        speed = stats.speed;
        jumpForce = stats.jumpForce;
        bossType = stats.bossType;
        interval = attackRange * 0.8f;
        
        movingTarget = target;
        SetMoveLock(false);
    }
    
    private void FixedUpdate()
    {
        if(movingTarget == null)
            return;
        
        CheckGround();
        CheckWall();
        CheckSide();
        lookSide = (movingTarget.transform.position.x > transform.position.x) ? 1f : -1f;
        
        if(_isMoveLocked)
            return;
        
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

        if (bossType == BossType.magician)
        {
            if (distance > (interval * interval) - distanceThreshold &&
                distance < (interval * interval) + distanceThreshold)
            {
                rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
            }
            else if (distance > (interval * interval))
            {
                rb2d.linearVelocity = new Vector2(direction * speed, rb2d.linearVelocity.y);
            }
            else
            {
                rb2d.linearVelocity = new Vector2(-direction * speed * reverseDeceleration, rb2d.linearVelocity.y);
            }
        }
        else
        {
            rb2d.linearVelocity = new Vector2(direction * speed, rb2d.linearVelocity.y);
        }
    }
    
    public void SetMoveLock(bool value)
    {
        _isMoveLocked = value;
        
        if (value)
        {
            rb2d.linearVelocity = new Vector2(0, 0);
        }
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

    public void TakeDamage(float damage){_bossHit.TakeDamage(damage);}

    public void ApplySlow(float percent, float duration) { }
}
