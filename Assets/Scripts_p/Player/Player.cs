using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;
    [SerializeField] private CharacterData character;
    [SerializeField] private PlayerAttack attacker;
    [SerializeField] private PlayerMove move;
    
    private Coroutine invincibleCoroutine;
    
    public float invincibilityDuration;
    public float currentGauge;
    public float maxGauge = 100f;
    public bool isInvincible;
    public bool isUsingUltimate;
    
    private void Start()
    {
        
        Init(character);
        UnLockedUlt();
    }
    public void Init(CharacterData data)
    {
        playerSkillExecutor.Init(data.Skills, data);
        move.Init(data.MoveSpeed, 13);
        attacker.Init(data);
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
        return true;
    }

    public void AddGauge(float amount)
    {
        currentGauge = Mathf.Clamp(currentGauge + amount, 0, maxGauge);
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;  
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

    
    
}
