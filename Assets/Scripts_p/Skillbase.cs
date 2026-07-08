using UnityEngine;

public abstract class SkillBase : ISkillAction
{
    protected CharacterData data;
    protected SkillData skillData;
    protected float lastUsedTime = -999f;

    public float cooldownReductionRate = 0f;

    public float Cooldown => Mathf.Max(0.1f, skillData.Cooldown * (1f - cooldownReductionRate));

    public void Init(CharacterData data, SkillData skillData)
    {
        this.data = data;
        this.skillData = skillData;
    }

    public bool CanUse()
    {
        return Time.time >= lastUsedTime + Cooldown;
    }

    public bool TryUse(GameObject user, Vector2 dir)
    {
        if (!CanUse())
        {
            float remain = (lastUsedTime + Cooldown) - Time.time;
            Debug.Log($"Skill cooldown: {remain:F2}");
            return false;
        }

        bool success = Execute(user, dir);
        if (success)
        {
            lastUsedTime = Time.time;
        }

        return success;
    }

    protected abstract bool Execute(GameObject user, Vector2 dir);
}
