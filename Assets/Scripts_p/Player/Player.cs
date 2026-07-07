using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour, IDamageable, IPlayerStatUp
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;
    [SerializeField] private CharacterData character;
    [SerializeField] private PlayerAttack attacker;
    [SerializeField] private PlayerMove move;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D bodyCollider;
    [SerializeField] private bool autoFitColliderToSprite = true;
    [SerializeField] private bool updateColliderOnEverySpriteChange;
    
    private Coroutine invincibleCoroutine;
    private Sprite lastColliderSprite;
    private PlayerAnimationClips currentAnimations;
    private AnimatorOverrideController currentOverrideController;
    private AnimationClip originalAttackClip;
    private AnimationClip currentAttackClip;
    private float facingDirection = 1f;
    private bool forcedIdle;

    public PlayerStats Stats { get; private set; } = new PlayerStats();
    
    public float invincibilityDuration;
    public float currentGauge;
    public float maxGauge = 100f;
    public bool isInvincible;
    public bool isUsingUltimate;
    public bool debugNoDamage;

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnGaugeChanged;
    public event Action OnDeath;

    private bool isDead;
    private bool hasSpeedParam;
    private bool hasMoveXParam;
    private bool hasYVelocityParam;
    private bool hasIsMovingParam;
    private bool hasIsGroundedParam;
    private bool hasAttackParam;
    private bool hasHitParam;
    private bool hasDieParam;
    private bool hasJumpParam;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            animator = spriteRenderer != null
                ? spriteRenderer.gameObject.AddComponent<Animator>()
                : gameObject.AddComponent<Animator>();

        if (bodyCollider == null)
            bodyCollider = GetComponent<BoxCollider2D>();

        if (playerSkillExecutor == null)
            playerSkillExecutor = GetComponent<PlayerSkillExecutor>();

        if (attacker == null)
            attacker = GetComponent<PlayerAttack>();

        if (move == null)
            move = GetComponent<PlayerMove>();

        CacheAnimatorParameters();
    }
    
    private void Start()
    {
        if (CharDataManager.Instance != null && CharDataManager.Instance.data != null)
            character = CharDataManager.Instance.data;

        if (character == null)
        {
            Debug.LogError("Player character data is missing.");
            return;
        }

        Init(character);
        UnLockedUlt();
    }

    private void OnEnable()
    {
        ArgumentDataManager.OnArgumentClicked += GetArgument;
    }

    private void OnDisable()
    {
        ArgumentDataManager.OnArgumentClicked -= GetArgument;
    }
    public void Init(CharacterData data)
    {
        character = data;
        isDead = false;
        currentAnimations = data.Animations;
        ApplyCharacterVisual(data);
        Stats.Init(data);
        playerSkillExecutor?.Init(data.Skills, data);
        move?.Init(this, 13);
        attacker?.Init(data);
        ArgumentDataManager.Instance?.GetJob(data.JobType);
        UIManager.Instance?.SetCharacter(data);
        NotifyHealthChanged();
        NotifyGaugeChanged();
    }

    public void UnLockedUlt()
    {
        StartCoroutine(GetGauge());
    }
    private IEnumerator GetGauge()
    {
        while (true)
        {
            AddGauge(1);
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator Invincibility(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        invincibleCoroutine = null;
    }
    public bool UseGauge(float amount)
    {
        if (currentGauge < amount)
            return false;

        currentGauge -= amount;
        NotifyGaugeChanged();
        return true;
    }

    public void AddGauge(float amount)
    {
        currentGauge = Mathf.Clamp(currentGauge + amount, 0, maxGauge);
        NotifyGaugeChanged();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (debugNoDamage) return;
        if (isInvincible) return;  
        Stats.currentHp = Mathf.Clamp(Stats.currentHp - amount, 0, Stats.MaxHp);
        CameraShake.Shake(0.24f, 0.18f);
        PlayHitAnimation();
        Debug.Log(Stats.currentHp);
        NotifyHealthChanged();

        if (Stats.currentHp <= 0f)
        {
            isDead = true;
            PlayDieAnimation();
            OnDeath?.Invoke();
            return;
        }

        StartInvincibility(invincibilityDuration);
    }

    public void ApplySlow(float percent, float duration)
    {
        if (isInvincible) return;
        move.Slow(percent, duration);
    }

    public void ApplyKnockback(Vector2 dir, float power, float duration)
    {
        if (isInvincible) return;
        move.KnockBack(dir, power, duration);
    }
    public void StopUltimate()
    {
        isUsingUltimate = false;
    }
    public void StartInvincibility(float duration)
    {
        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);

        invincibleCoroutine = StartCoroutine(Invincibility(duration));
    }

    private void GetArgument(ArgumentResult result)
    {
        switch (result.kind)
        {
            case ArgumentKind.Skill:
                break;
            case ArgumentKind.Stat:
                Stats.ApplyStat(result);
                NotifyHealthChanged();
                break;
            default:
                Debug.Log("에러발생 : 증강데이터 타입 소실");
                break;
        }
    }
    

    public void StatUp()
    {
        float prevMaxHp = Stats.MaxHp;
        float prevDmg  = Stats.Damage;
        float prevSpd  = Stats.MoveSpeed;
        float prevASpd = Stats.AttackSpeed;
        float prevRange = Stats.Range;
        
        Stats.AddMoveSpeed(character.RisingMoveSpeed);
        Stats.AddDamage(character.RisingDamage);
        Stats.AddRange(character.RisingRange);
        Stats.AddAttackSpeed(character.RisingAttackSpeed);
        Stats.AddMaxHp(character.RisingMaxHp);
        NotifyHealthChanged();
        
        Debug.Log($"<color=#FFD700><b>[Level Up!]</b></color> 캐릭터 스탯이 상승했습니다.");
    
        string log = $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                     $"[HP] : {prevMaxHp:F1} ➔ <color=#FF4444>{Stats.MaxHp:F1}</color> (+{character.RisingMaxHp})\n" +
                     $"[공격력] : {prevDmg:F1} ➔ <color=#FF4444>{Stats.Damage:F1}</color> (+{character.RisingDamage * 100}%)\n" +
                     $"[이동속도] : {prevSpd:F1} ➔ <color=#4444FF>{Stats.MoveSpeed:F1}</color> (+{character.RisingMoveSpeed * 100}%)\n" +
                     $"[공격속도] : {prevASpd:F1} ➔ <color=#4444FF>{Stats.AttackSpeed:F1}</color> (+{character.RisingAttackSpeed * 100}%)\n" +
                     $"[사거리] : {prevRange:F1} ➔ <color=#FFFF44>{Stats.Range:F1}</color> (+{character.RisingRange * 100}%)\n" +
                     $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";
    
        Debug.Log(log);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            StatUp();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            debugNoDamage = !debugNoDamage;
            Debug.Log($"Debug no damage: {debugNoDamage}");
        }
    }

    private void LateUpdate()
    {
        if (updateColliderOnEverySpriteChange)
            UpdateSpriteColliderIfNeeded();
    }

    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(Stats.currentHp, Stats.MaxHp);
    }

    private void NotifyGaugeChanged()
    {
        OnGaugeChanged?.Invoke(currentGauge, maxGauge);
    }

    private void ApplyCharacterVisual(CharacterData data)
    {
        if (data == null)
            return;

        if (animator != null && data.AnimatorController != null)
        {
            animator.runtimeAnimatorController = data.AnimatorController;
            ApplyCharacterAnimationClips(data);
            CacheAnimatorParameters();
        }
        else if (data.AnimatorController == null)
        {
            Debug.LogWarning($"{data.name} CharacterData에 AnimatorController가 비어 있습니다.");
        }

        if (spriteRenderer != null && data.DefaultSprite != null)
            spriteRenderer.sprite = data.DefaultSprite;

        UpdateSpriteCollider();
    }

    private void UpdateSpriteColliderIfNeeded()
    {
        if (!autoFitColliderToSprite || spriteRenderer == null || spriteRenderer.sprite == lastColliderSprite)
            return;

        UpdateSpriteCollider();
    }

    private void UpdateSpriteCollider()
    {
        if (!autoFitColliderToSprite || spriteRenderer == null || bodyCollider == null || spriteRenderer.sprite == null)
            return;

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector3 worldCenter = spriteRenderer.transform.TransformPoint(spriteBounds.center);
        Vector3 worldSize = Vector3.Scale(spriteBounds.size, spriteRenderer.transform.lossyScale);
        Vector3 localCenter = bodyCollider.transform.InverseTransformPoint(worldCenter);
        Vector3 localSize = bodyCollider.transform.InverseTransformVector(worldSize);

        bodyCollider.offset = localCenter;
        bodyCollider.size = new Vector2(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y));
        lastColliderSprite = spriteRenderer.sprite;
    }

    private void ApplyCharacterAnimationClips(CharacterData data)
    {
        if (animator == null || data.Animations == null || !data.Animations.HasAnyClip())
            return;

        AnimatorOverrideController overrideController = new AnimatorOverrideController(data.AnimatorController);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);
        originalAttackClip = null;
        currentAttackClip = null;

        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip originalClip = overrides[i].Key;

            if (originalClip == null)
                continue;

            if (originalClip.name == "Attack")
                originalAttackClip = originalClip;

            AnimationClip replacementClip = data.Animations.GetClipOrFallback(originalClip.name);

            if (replacementClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, replacementClip);

                if (originalClip.name == "Attack")
                    currentAttackClip = replacementClip;
            }
        }

        overrideController.ApplyOverrides(overrides);
        currentOverrideController = overrideController;
        animator.runtimeAnimatorController = overrideController;
    }

    public void UpdateMoveAnimation(Vector2 velocity, Vector2 input, bool isGrounded)
    {
        if (animator == null)
            return;

        float animationYVelocity = isGrounded ? 0f : velocity.y;
        bool isMovingHorizontally = Mathf.Abs(input.x) > 0.01f && Mathf.Abs(velocity.x) > 0.05f;

        if (hasSpeedParam)
            animator.SetFloat(SpeedHash, isMovingHorizontally ? Mathf.Abs(velocity.x) : 0f);

        if (hasMoveXParam)
            animator.SetFloat(MoveXHash, input.x);

        if (hasYVelocityParam)
            animator.SetFloat(YVelocityHash, animationYVelocity);

        if (hasIsMovingParam)
            animator.SetBool(IsMovingHash, isMovingHorizontally);

        if (hasIsGroundedParam)
            animator.SetBool(IsGroundedHash, isGrounded);

        ForceIdleWhenStopped(isMovingHorizontally, isGrounded);
    }

    private void ForceIdleWhenStopped(bool isMovingHorizontally, bool isGrounded)
    {
        if (isMovingHorizontally)
        {
            forcedIdle = false;
            return;
        }

        if (!isGrounded || isMovingHorizontally || animator == null || animator.runtimeAnimatorController == null)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!forcedIdle && stateInfo.IsName("Run"))
        {
            animator.CrossFade("Idle", 0f);
            forcedIdle = true;
        }
    }

    public void SetFacingDirection(float direction)
    {
        if (spriteRenderer == null || Mathf.Abs(direction) < 0.01f)
            return;

        facingDirection = Mathf.Sign(direction);
        spriteRenderer.flipX = facingDirection < 0f;
    }

    public void PlayAttackAnimation(int comboIndex = 0)
    {
        if (animator == null || !hasAttackParam || currentAnimations == null || !currentAnimations.HasAttackClip())
            return;

        ApplyAttackAnimationClip(comboIndex);
        animator.SetTrigger(AttackHash);
    }

    private void ApplyAttackAnimationClip(int comboIndex)
    {
        if (currentOverrideController == null || originalAttackClip == null || currentAnimations == null)
            return;

        AnimationClip attackClip = currentAnimations.GetAttackClip(comboIndex);

        if (attackClip == null || attackClip == currentAttackClip)
            return;

        currentOverrideController[originalAttackClip] = attackClip;
        currentAttackClip = attackClip;
    }

    public void PlayJumpAnimation()
    {
        if (animator != null && hasJumpParam && (currentAnimations == null || currentAnimations.HasAnyJumpClip()))
            animator.SetTrigger(JumpHash);
    }

    private void PlayHitAnimation()
    {
        if (animator != null && hasHitParam)
            animator.SetTrigger(HitHash);
    }

    private void PlayDieAnimation()
    {
        if (animator != null && hasDieParam)
            animator.SetTrigger(DieHash);
    }

    private void CacheAnimatorParameters()
    {
        hasSpeedParam = HasAnimatorParameter(SpeedHash);
        hasMoveXParam = HasAnimatorParameter(MoveXHash);
        hasYVelocityParam = HasAnimatorParameter(YVelocityHash);
        hasIsMovingParam = HasAnimatorParameter(IsMovingHash);
        hasIsGroundedParam = HasAnimatorParameter(IsGroundedHash);
        hasAttackParam = HasAnimatorParameter(AttackHash);
        hasHitParam = HasAnimatorParameter(HitHash);
        hasDieParam = HasAnimatorParameter(DieHash);
        hasJumpParam = HasAnimatorParameter(JumpHash);
    }

    private bool HasAnimatorParameter(int parameterHash)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash)
                return true;
        }

        return false;
    }
}
