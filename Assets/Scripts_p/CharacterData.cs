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

    [Header("Animation")]
    public RuntimeAnimatorController AnimatorController;
    public Sprite DefaultSprite;
    
    [Header("올라가는 스탯")]
    public float RisingMoveSpeed;
    public float RisingDamage;
    public float RisingRange;
    public float RisingAttackSpeed;
    public float RisingMaxHp;

}
