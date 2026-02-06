using UnityEngine;

public enum ArgumentType
{
    Health,
    Attack,
    AttackSpeed,
    Speed,
    Range,
    Skill,
}
[CreateAssetMenu(fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string name;
    public Sprite icon;
    public string description;
    public ArgumentType argumentType;
}
