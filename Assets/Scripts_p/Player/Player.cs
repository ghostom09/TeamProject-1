using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour, IDamageable, IPlayerStatUp
{
    
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;
    [SerializeField] private CharacterData character;
    [SerializeField] private PlayerAttack attacker;
    [SerializeField] private PlayerMove move;
    
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
        Debug.Log(Stats.currentHp);
        NotifyHealthChanged();

        if (Stats.currentHp <= 0f)
        {
            isDead = true;
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
}
