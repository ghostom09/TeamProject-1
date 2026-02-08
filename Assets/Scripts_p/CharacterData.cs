using UnityEngine;

public enum jobType
{
    Sword,
    Gun,
}
[CreateAssetMenu(menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public jobType JobType;
    public float MoveSpeed;
    public float Damage;
    public float Hp;
    public float Range;
    public float AttackSpeed;
    public SkillData[] Skills;
    public UltData Ult;
}
