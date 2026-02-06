using UnityEngine;

[CreateAssetMenu(fileName = "SkillData")]
public abstract class SkillData : ScriptableObject
{
    public string skillName;
    public float cooldown;
    public abstract void Execute(GameObject owner, Vector2 mouseAngle);
}
