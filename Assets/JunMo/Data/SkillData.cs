using UnityEngine;

[CreateAssetMenu(fileName = "SkillData")]
public class SkillData : ScriptableObject
{
    public SkillType SkillName;
    public float Cooldown;
}