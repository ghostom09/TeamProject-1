using System.Collections;
using System;
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
    
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;
    [SerializeField] private CharacterData character;
    [SerializeField] private PlayerAttack attacker;
    [SerializeField] private PlayerMove move;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Coroutine invincibleCoroutine;

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

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        ApplyCharacterVisual(data);
        Stats.Init(data);
        playerSkillExecutor.Init(data.Skills, data);
        move.Init(this, 13);
        attacker.Init(data);
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
            CacheAnimatorParameters();
        }

        if (spriteRenderer != null && data.DefaultSprite != null)
            spriteRenderer.sprite = data.DefaultSprite;
    }

    public void UpdateMoveAnimation(Vector2 velocity, Vector2 input, bool isGrounded)
    {
        if (animator == null)
            return;

        if (hasSpeedParam)
            animator.SetFloat(SpeedHash, Mathf.Abs(velocity.x));

        if (hasMoveXParam)
            animator.SetFloat(MoveXHash, input.x);

        if (hasYVelocityParam)
            animator.SetFloat(YVelocityHash, velocity.y);

        if (hasIsMovingParam)
            animator.SetBool(IsMovingHash, Mathf.Abs(input.x) > 0.01f);

        if (hasIsGroundedParam)
            animator.SetBool(IsGroundedHash, isGrounded);
    }

    public void PlayAttackAnimation()
    {
        if (animator != null && hasAttackParam)
            animator.SetTrigger(AttackHash);
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
