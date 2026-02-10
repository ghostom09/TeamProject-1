using UnityEngine;
// using UnityEngine.UI;

public enum ItemType
{
    Stat,
    Skill,
}
public enum StatType
{
    Health,
    Attack,
    AttackSpeed,
    Speed,
    Range,
}

public enum SkillType
{
    SwordMove,
    SwordCC,
    SwordUltra,
    GunShot,
    GunTrigger,
    GunUltra,
}
[CreateAssetMenu(fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;

    public float value;
    
    public ItemType itemType;
    public StatType statType;
    public SkillType skillType;
}
