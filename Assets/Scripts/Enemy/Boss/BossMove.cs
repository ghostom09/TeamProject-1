using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BossMove : MonoBehaviour, IBossReset, IEnemyMover, IDamageable
{
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private const float DefaultJumpForce = 13f;

    [SerializeField] private Animator animator;
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
    [SerializeField] private bool passThroughThinPlatforms = true;
    [SerializeField] private float platformMaxHeight = 0.5f;
    [SerializeField] private float platformStandTolerance = 0.12f;
    public bool isGrounded;
    private bool isleftWall;
    private bool isrightWall;

    private bool leftSide;
    private bool rightSide;
    private bool isSide;
    private bool isJumping = false;
    
    [SerializeField] private float interval;
    [SerializeField] private float jumpPersent = 0.015f;
    [SerializeField] private float higherTargetJumpCooldown = 1.2f;
    [SerializeField] private float higherTargetMinY = 0.25f;
    [SerializeField] private float reverseDeceleration;

    private float distance;
    private float direction;
    private float deltaX;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private float distanceThreshold = 0.1f;
    
    [SerializeField] private bool _isMoveLocked;
    public float lookSide;
    private float nextHigherTargetJumpTime;
    
    private Coroutine knockRoutine;

    private BossHit _bossHit;
    private SpriteRenderer spriteRenderer;
    private Collider2D bodyCollider;
    private readonly List<Collider2D> ignoredPlatformColliders = new List<Collider2D>();
    private bool hasWalkParam;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _bossHit = GetComponent<BossHit>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<Collider2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        hasWalkParam = HasAnimatorParameter(WalkHash);
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        hasWalkParam = HasAnimatorParameter(WalkHash);

        speed = stats.speed;
        jumpForce = stats.jumpForce > 0f ? stats.jumpForce : jumpForce;
        if (jumpForce <= 0f)
            jumpForce = DefaultJumpForce;
        bossType = stats.bossType;
        foreach (var skill in stats.skills)
        {
            if(skill.skillType == BossSkillType.Normal)
                interval = skill.attackRange * 0.8f;
        }
        
        movingTarget = target;
        SetMoveLock(false);
    }
    
    private void FixedUpdate()
    {
        UpdatePlatformCollisionState();

        if(movingTarget == null)
        {
            UpdateAnimator();
            return;
        }
        
        CheckGround();
        CheckWall();
        CheckSide();
        lookSide = (movingTarget.transform.position.x > transform.position.x) ? 1f : -1f;
        UpdateFacing();

        if(_isMoveLocked)
        {
            UpdateAnimator();
            return;
        }

        if (ShouldJumpToHigherTarget() && CanStartJump())
            verticalmove();
        
        horizontalmove();
        
        if (isGrounded)
        {
            isJumping = false;
            if (isleftWall || isrightWall)
            {
                verticalmove();
            }
        }

        UpdateAnimator();
    }

    private void UpdateFacing()
    {
        if (spriteRenderer == null)
            return;

        float faceDirection = bossType == BossType.magician
            ? lookSide
            : Mathf.Abs(rb2d.linearVelocity.x) > 0.01f
                ? rb2d.linearVelocity.x
                : lookSide;

        spriteRenderer.flipX = faceDirection < 0f;
    }
    
    private void CheckWall()
    {
        isleftWall = HasBlockingWall(
            leftWallCheck.position,
            groundRadius
        );
        isrightWall = HasBlockingWall(
            rightWallCheck.position,
            groundRadius
        );
    }

    private void CheckGround()
    {
        isGrounded = HasGround(
            groundCheck.position,
            groundRadius
        );
    }

    private void CheckSide()
    {
        leftSide = HasGround(leftSideGroundCheck.position, sideRadius);
        rightSide = HasGround(rightSideGroundCheck.position, sideRadius);
        
        isSide = leftSide ^ rightSide;
    }

    private bool HasGround(Vector2 position, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, wallLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit == bodyCollider)
                continue;

            if (!IsPassThroughPlatform(hit) || CanStandOnPlatform(hit))
                return true;
        }

        return false;
    }

    private bool HasBlockingWall(Vector2 position, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, wallLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit == bodyCollider)
                continue;

            if (!IsPassThroughPlatform(hit))
                return true;
        }

        return false;
    }

    private bool IsPassThroughPlatform(Collider2D targetCollider)
    {
        if (!passThroughThinPlatforms || targetCollider == null || targetCollider.isTrigger)
            return false;

        int targetLayerMask = 1 << targetCollider.gameObject.layer;
        if ((wallLayer.value & targetLayerMask) == 0)
            return false;

        Bounds bounds = targetCollider.bounds;
        return bounds.size.y <= platformMaxHeight && bounds.size.x > bounds.size.y;
    }

    private bool CanStandOnPlatform(Collider2D platformCollider)
    {
        if (platformCollider == null || bodyCollider == null || rb2d == null)
            return false;

        Bounds bodyBounds = bodyCollider.bounds;
        Bounds platformBounds = platformCollider.bounds;
        bool overlapsHorizontally =
            bodyBounds.max.x > platformBounds.min.x + 0.03f &&
            bodyBounds.min.x < platformBounds.max.x - 0.03f;
        bool isAbovePlatform = bodyBounds.min.y >= platformBounds.max.y - platformStandTolerance;
        bool isFallingOrResting = rb2d.linearVelocity.y <= 0.05f;

        return overlapsHorizontally && isAbovePlatform && isFallingOrResting;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryIgnorePlatformCollision(GetOtherCollisionCollider(collision));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryIgnorePlatformCollision(GetOtherCollisionCollider(collision));
    }

    private Collider2D GetOtherCollisionCollider(Collision2D collision)
    {
        if (collision.collider == bodyCollider)
            return collision.otherCollider;

        return collision.collider;
    }

    private void TryIgnorePlatformCollision(Collider2D other)
    {
        if (!IsPassThroughPlatform(other) || CanStandOnPlatform(other))
            return;

        SetPlatformIgnored(other, true);
    }

    private void UpdatePlatformCollisionState()
    {
        if (bodyCollider == null)
            return;

        for (int i = ignoredPlatformColliders.Count - 1; i >= 0; i--)
        {
            Collider2D platform = ignoredPlatformColliders[i];
            if (platform == null)
            {
                ignoredPlatformColliders.RemoveAt(i);
                continue;
            }

            if (CanStandOnPlatform(platform))
                SetPlatformIgnored(platform, false);
        }
    }

    private void SetPlatformIgnored(Collider2D platform, bool ignored)
    {
        if (bodyCollider == null || platform == null)
            return;

        Physics2D.IgnoreCollision(bodyCollider, platform, ignored);

        if (ignored)
        {
            if (!ignoredPlatformColliders.Contains(platform))
                ignoredPlatformColliders.Add(platform);
        }
        else
        {
            ignoredPlatformColliders.Remove(platform);
        }
    }

    private void OnDisable()
    {
        RestoreIgnoredPlatformCollisions();
    }

    private void OnDestroy()
    {
        RestoreIgnoredPlatformCollisions();
    }

    private void RestoreIgnoredPlatformCollisions()
    {
        if (bodyCollider == null)
        {
            ignoredPlatformColliders.Clear();
            return;
        }

        foreach (Collider2D platform in ignoredPlatformColliders)
        {
            if (platform != null)
                Physics2D.IgnoreCollision(bodyCollider, platform, false);
        }

        ignoredPlatformColliders.Clear();
    }
    
    private void verticalmove()
    {
        nextHigherTargetJumpTime = Time.time + higherTargetJumpCooldown;
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        isJumping = true;
    }

    private bool ShouldJumpToHigherTarget()
    {
        float targetHeightDifference = movingTarget.transform.position.y - transform.position.y;
        if (targetHeightDifference < higherTargetMinY)
            return false;

        if (Time.time < nextHigherTargetJumpTime)
            return false;

        return isSide || Random.value <= jumpPersent;
    }

    private bool CanStartJump()
    {
        return isGrounded || Mathf.Abs(rb2d.linearVelocity.y) <= 0.05f;
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
    
    public void SetMoveLock(bool value)
    {
        _isMoveLocked = value;
        
        if (value)
        {
            rb2d.linearVelocity = new Vector2(0, 0);
        }

        UpdateAnimator();
    }
    
    public void ApplyKnockback(Vector2 dir, float power, float duration)
    {
        if(_isMoveLocked)
            return;
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

    private void UpdateAnimator()
    {
        if (animator == null || !hasWalkParam)
            return;

        bool isWalking = bossType == BossType.warrior &&
                         !_isMoveLocked &&
                         Mathf.Abs(rb2d.linearVelocity.x) > 0.01f;

        animator.SetBool(WalkHash, isWalking);
    }

    private bool HasAnimatorParameter(int parameterHash)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash)
                return true;
        }

        return false;
    }
}
