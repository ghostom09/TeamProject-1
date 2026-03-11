using UnityEngine;

public enum BossSkillType
{
    None,
    Normal,
    longDistance,
    middleDistance,
    shortDistance,
}

[CreateAssetMenu(fileName = "BossSkills")]

public class BossSkills : ScriptableObject
{
    public BossSkillType skills;
    public float cooldown;
    public float attackRange;
    public float damage;
}
