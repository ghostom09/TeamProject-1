using UnityEngine;

[CreateAssetMenu(fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string name;
    public Sprite icon;
    public string description;
    public ArgumentStatData[] stats;
    // public SkillData[] skills;
    
}
