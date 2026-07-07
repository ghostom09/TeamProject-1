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
    private PlayerLevelManager _levelManager;

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
            { SkillType.GunUlt, new GunUlt() }
        };

        this.data = data;
        _levelManager = GetComponent<PlayerLevelManager>();
        
        foreach (var skillData in _skills)
        {
            SkillType type = skillData.SkillName;
            _actions[type].Init(data, skillData);
        }

        RefreshSkillCooldownUI();
    }

    public void GetDirection(int index)
    {
        Vector2 mouseScreen = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        
        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        
        UseSkill(index, dir);
    }
    public void UseSkill(int index, Vector2 dir)
    {
        if (_skills == null || index < 0 || index >= _skills.Length)
            return;

        SkillType type = _skills[index].SkillName;
        if (!CanUseSkill(index, type))
            return;

        if (_actions[type].TryUse(gameObject, dir))
        {
            UIManager.Instance?.UpdateSkillTimer(index);
        }
    }

    private bool CanUseSkill(int index, SkillType type)
    {
        // if (index == 0)
        //     return true;
        //
        // if (ArgumentDataManager.Instance == null || _levelManager == null)
        //     return true;
        //
        // int unlockLevel = ArgumentDataManager.Instance.GetSkillUnlockLevel(type);
        // return _levelManager.CurrentLevel >= unlockLevel;
        return true;
    }

    public SkillBase GetSkill(SkillType type)
    {
        return _actions[type] as SkillBase;
    }

    public void RefreshSkillCooldownUI()
    {
        if (_skills == null || _actions == null)
            return;

        float[] cooldowns = new float[_skills.Length];
        for (int i = 0; i < _skills.Length; i++)
        {
            SkillType type = _skills[i].SkillName;
            SkillBase skill = _actions[type] as SkillBase;
            cooldowns[i] = skill != null ? skill.Cooldown : _skills[i].Cooldown;
        }

        UIManager.Instance?.SetSkillCooldowns(cooldowns);
    }
}
