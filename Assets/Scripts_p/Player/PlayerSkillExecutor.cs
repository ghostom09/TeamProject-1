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

    private void OnEnable()
    {
        PlayerLevelManager.OnLevelUp += OnPlayerLevelUp;
    }

    private void OnDisable()
    {
        PlayerLevelManager.OnLevelUp -= OnPlayerLevelUp;
    }

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
        RefreshSkillUnlockUI();
    }

    private void OnPlayerLevelUp(int level)
    {
        RefreshSkillUnlockUI();
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
        if (_levelManager == null)
            return true;

        int unlockLevel = GetSkillUnlockLevel(type);
        bool isUnlocked = IsSkillUnlocked(type);

        if (!isUnlocked)
            Debug.Log($"Skill {type} unlocks at level {unlockLevel}. Current level: {_levelManager.CurrentLevel}");

        return isUnlocked;
    }

    private bool IsSkillUnlocked(SkillType type)
    {
        if (_levelManager == null)
            return true;

        return _levelManager.CurrentLevel >= GetSkillUnlockLevel(type);
    }

    private int GetSkillUnlockLevel(SkillType type)
    {
        if (ArgumentDataManager.Instance != null)
            return ArgumentDataManager.Instance.GetSkillUnlockLevel(type);

        return type switch
        {
            SkillType.Sword1 => 6,
            SkillType.Sword2 => 3,
            SkillType.SwordUlt => 10,
            SkillType.Gun1 => 3,
            SkillType.Gun2 => 6,
            SkillType.GunUlt => 10,
            _ => 1
        };
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

    private void RefreshSkillUnlockUI()
    {
        if (_skills == null)
            return;

        bool[] unlocks = new bool[_skills.Length];
        for (int i = 0; i < _skills.Length; i++)
        {
            unlocks[i] = IsSkillUnlocked(_skills[i].SkillName);
        }

        UIManager.Instance?.SetSkillUnlocks(unlocks);
    }
}
