using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour, IEnemyMover
{
    [SerializeField] private GameObject target;
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
    
    private float groundRadius = 0.2f;
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

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Init(EnemyStats stats)
    {
        speed = stats.speed;
        jumpForce = stats.jumpForce;
        enemyType = stats.enemyType;
        attackRange = stats.attackRange;
        rangedInterval = attackRange * 0.8f;
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
                if (target.transform.position.y > transform.position.y)
                {
                    jumpTry = Random.Range(50, 100);
                    if(jumpTry == 90)
                        verticalmove();
                }
            }
        }
        else if(!isJumping&&!isGrounded&&target.transform.position.y > transform.position.y)
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
        distance = ((target.transform.position.x - transform.position.x) *
                    (target.transform.position.x - transform.position.x)) +
                   ((target.transform.position.y - transform.position.y) *
                    (target.transform.position.y - transform.position.y));

        direction = target.transform.position.x - transform.position.x;

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
}
