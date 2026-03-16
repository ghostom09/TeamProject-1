using UnityEngine;

public enum BossSkillType
{
    None,
    Normal,
    longDistance,
    shortDistance,
    passive,
    ultimate
}

[CreateAssetMenu(fileName = "BossSkills")]

public class BossSkills : ScriptableObject
{
    public BossSkillType skillType;
    public float cooldown;
    public float attackRange;
    public float damage;
}
