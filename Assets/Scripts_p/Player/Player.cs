using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;
    [SerializeField] private CharacterData character;
    [SerializeField] private PlayerAttack attacker;
    
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
        attacker.Init(data);
    }

    public void UnLockedUlt()
    {
        StartCoroutine(GetGauge());
    }
    private void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(0,dir);
        }else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(1,dir);
        }else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(2,dir);
        }
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
        StartCoroutine(Invincibility(invincibilityDuration));
    }

    public void ApplySlow(float percent, float duration)
    {
        if (isInvincible) return;
    }

    public void ApplyKnockback(Vector2 dir, float power, float duration)
    {
        if (isInvincible) return;
    }
    public void StopUltimate()
    {
        isUsingUltimate = false;
    }
    
}
