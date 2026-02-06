using UnityEngine;

[CreateAssetMenu(fileName = "SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float cooldown;
    // public abstract void Execute(GameObject owner);
}
