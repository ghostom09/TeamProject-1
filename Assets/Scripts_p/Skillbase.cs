
using UnityEngine;

public abstract class SkillBase : ISkillAction
{
    protected float cooldown;
    private float lastUsedTime = -999f;

    protected SkillBase(float cooldown)
    {
        this.cooldown = cooldown;
    }

    public bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void TryUse(GameObject user, Vector2 dir)
    {
        if (!CanUse())
        {
            float remain = cooldown - (Time.time - lastUsedTime);
            Debug.Log($"남은 쿨타임: {remain:F2}");
            return;
        }
            

        lastUsedTime = Time.time;
        Execute(user, dir);
    }

    protected abstract void Execute(GameObject user, Vector2 dir);
}
