using UnityEngine;
using System;

[Serializable]
public struct ArgumentResult
{
    public ArgumentKind kind;

    public StatType statType;
    public float statValue;

    public SkillType skillType;
    public int skillLevel;
}

public enum StatType
{
    Health,
    Attack,
    AttackSpeed,
    Speed,
    Range,
    Heal
}

public abstract class ArgumentData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;

    public int weight = 10;

    public jobType[] allowedJobs;

    public bool IsAllowed(jobType job)
    {
        if (allowedJobs == null || allowedJobs.Length == 0)
            return true;

        foreach (var j in allowedJobs)
        {
            if (j == job)
                return true;
        }

        return false;
    }
}