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

    public void Init(SkillData[] skills)
    {
        _skills = skills;
    }

    public void UseSkill(int index, Vector2 dir)
    {
        SkillData skill = _skills[index];
        ExecuteSkill(skill, dir);
    }


    public void ExecuteSkill(SkillData data, Vector2 dir)
    {
        Debug.Log(data.SkillName + "발동!!!!!1");
    }
}
