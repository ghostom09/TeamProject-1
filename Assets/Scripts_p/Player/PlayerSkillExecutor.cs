using System.Collections.Generic;
using UnityEngine;


public enum SkillType
{
    Sword1,
    Sword2,
    SwordUlt,
    Gun1,
    Gun2,
    GunUlt
}
public class PlayerSkillExecutor : MonoBehaviour
{
    private SkillData[] _skills;
    private Dictionary<SkillType, ISkillAction> _actions;

    private CharacterData data;

    public void Init(SkillData[] skills, CharacterData data)
    {
        _skills = skills;
        
        _actions = new()
        {
            { SkillType.Sword1, new Sword1() },
            { SkillType.Sword2, new Sword2() },
            { SkillType.SwordUlt, new SwordUlt() },
            { SkillType.Gun1, new Gun1() },
            { SkillType.Gun2, new Gun2() },
            { SkillType.GunUlt, new GunUlt()}
        };

        this.data = data;
        
        Debug.Log($"{_skills[0].SkillName}, {_skills[1].SkillName} ,{_skills[2].SkillName}");

    }

    public void UseSkill(int index, Vector2 dir)
    {
        SkillType type = _skills[index].SkillName;
        _actions[type].Init(data,_skills[index]);
        _actions[type].TryUse(gameObject, dir);
    }
    public SkillBase GetSkill(SkillType type)
    {
        return _actions[type] as SkillBase;
    }
}
