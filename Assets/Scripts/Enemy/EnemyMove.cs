using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform leftwallCheck;
    [SerializeField] private Transform rightwallCheck;
    
    private Rigidbody2D rb2d;
    private Enemy enemyStat;
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

    

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        enemyStat = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        speed = enemyStat.stats.speed;
        jumpForce = enemyStat.stats.jumpForce;
        enemyType = enemyStat.stats.enemyType;
        attackRange = enemyStat.stats.attackRange;
        rangedInterval = attackRange * 0.8f;
    }
    
    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
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
                if (player.transform.position.y > transform.position.y)
                {
                    jumpTry = Random.Range(50, 100);
                    if(jumpTry == 90)
                        verticalmove();
                }
            }
        }
        else if(!isJumping&&!isGrounded&&player.transform.position.y > transform.position.y)
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
        distance = ((player.transform.position.x - transform.position.x) *
                    (player.transform.position.x - transform.position.x)) +
                   ((player.transform.position.y - transform.position.y) *
                    (player.transform.position.y - transform.position.y));

        direction = player.transform.position.x - transform.position.x;

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
}
