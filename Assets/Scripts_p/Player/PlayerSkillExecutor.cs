using System.Collections.Generic;
using UnityEngine;


public enum SkillType
{
    Sword1,
    Sword2,
    Gun1,
    Gun2,
}
public class PlayerSkillExecutor : MonoBehaviour
{
    private SkillData[] _skills;
    private Dictionary<SkillType, ISkillAction> _actions;

    public void Init(SkillData[] skills)
    {
        _skills = skills;
        
        _actions = new()
        {
            { SkillType.Sword1, new Sword1() },
            { SkillType.Sword2, new Sword2() },
            { SkillType.Gun1, new Gun1() },
            { SkillType.Gun2, new Gun2() }
        };
    }

    public void UseSkill(int index, Vector2 dir)
    {
        SkillType type = _skills[index].SkillName;

        _actions[type].TryUse(gameObject, dir);
    }
}
