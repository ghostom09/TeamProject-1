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
    [SerializeField] private float jumpForce;
    private int jumpTry = 0;
    
    private float groundRadius = 0.2f;
    private bool isGrounded;
    private bool isleftWall;
    private bool isrightWall;
    private bool isJumping = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        enemyStat = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        speed = enemyStat.stats.speed;
        jumpForce = enemyStat.stats.jumpForce;
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
        if (player.transform.position.x < transform.position.x)
        {
            rb2d.linearVelocity = new Vector2(-speed, rb2d.linearVelocity.y);
        }
        else
        {
            rb2d.linearVelocity = new Vector2(speed, rb2d.linearVelocity.y);
        }
    }
}
