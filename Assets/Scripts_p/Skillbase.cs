// SkillBase.cs 개선안
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

    public void TryUse(GameObject user, Vector2 dir)
    {
        if (!CanUse())
        {
            float remain = (lastUsedTime + Cooldown) - Time.time;
            Debug.Log($"남은 쿨타임: {remain:F2}");
            return;
        }
        
        // Execute가 성공적으로 실행되었을 때만 쿨타임을 돌림
        bool success = Execute(user, dir);
        if (success)
        {
            lastUsedTime = Time.time; // 여기서 한 번만 처리! 자식 클래스에서 신경 쓸 필요 없음.
        }
    }

    // void 대신 bool을 반환하도록 변경
    protected abstract bool Execute(GameObject user, Vector2 dir);
}