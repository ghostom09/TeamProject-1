
using UnityEngine;

public abstract class SkillBase : ISkillAction
{
    protected float cooldown;
    protected CharacterData data;
    protected float lastUsedTime = -999f;
    protected float baseCooldown;
    protected float bonusCooldown;

    public float Cooldown => Mathf.Max(0.1f, baseCooldown + bonusCooldown);


    public void Init(CharacterData data)
    {
        this.data = data;
    }
    public void AddCooldownBonus(float value)
    {
        bonusCooldown += value;
    }
    public void ResetBonus()
    {
        bonusCooldown = 0;
    }
    protected SkillBase(float cooldown)
    {
        baseCooldown = cooldown;
    }

    public bool CanUse()
    {
        return Time.time >= lastUsedTime + Cooldown;
    }

    public void TryUse(GameObject user, Vector2 dir)
    {
        if (!CanUse())
        {
            float remain = Cooldown - (Time.time - lastUsedTime);
            Debug.Log($"남은 쿨타임: {remain:F2}");
            return;
        }
        Execute(user, dir);
    }
    protected abstract void Execute(GameObject user, Vector2 dir);
}
