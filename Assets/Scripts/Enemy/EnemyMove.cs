using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour, IEnemyMover, IDamageable, IEnemyReset
{
    private GameObject movingTarget;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform leftwallCheck;
    [SerializeField] private Transform rightwallCheck;
    
    private Rigidbody2D rb2d;
    private float speed;
    private float attackRange;
    private EnemyType enemyType;
    [SerializeField] private float jumpForce;
    private int jumpTry = 0;
    
    private float groundRadius = 0.35f;
    private bool isGrounded;
    private bool isleftWall;
    private bool isrightWall;
    private bool isJumping = false;
    
    private float rangedInterval;
    [SerializeField] private float reverseDeceleration;

    private float distance;
    private float direction;
    private float deltaX;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private float distanceThreshold = 0.1f;
    
    private bool _isMoveLocked;
    
    private Coroutine knockRoutine;

    private EnemyHit _enemyHit;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _enemyHit = GetComponent<EnemyHit>();
    }

    public void Init(EnemyStats stats, GameObject target)
    {
        speed = stats.speed;
        jumpForce = stats.jumpForce;
        enemyType = stats.enemyType;
        attackRange = stats.attackRange;
        rangedInterval = attackRange * 0.8f;
        
        movingTarget = target;
    }
    
    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        
        if(_isMoveLocked)
            return;
        
        horizontalmove();
        
        if (isGrounded)
        {
            isJumping = false;
            if (isleftWall || isrightWall)
            {
                verticalmove();
            }
            else
            {
                if (movingTarget.transform.position.y > transform.position.y)
                {
                    jumpTry = Random.Range(50, 100);
                    if(jumpTry == 90)
                        verticalmove();
                }
            }
        }
        else if(!isJumping&&!isGrounded&&movingTarget.transform.position.y > transform.position.y)
        {
            verticalmove();
        }
    }
    
    private void CheckWall()
    {
        isleftWall = Physics2D.OverlapCircle(
            leftwallCheck.position,
            groundRadius,
            wallLayer
        );
        isrightWall = Physics2D.OverlapCircle(
            rightwallCheck.position,
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

        if (enemyType == EnemyType.Ranged)
        {
            if (distance > (rangedInterval * rangedInterval) - distanceThreshold &&
                distance < (rangedInterval * rangedInterval) + distanceThreshold)
            {
                rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
            }
            else if (distance > (rangedInterval * rangedInterval))
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
}
