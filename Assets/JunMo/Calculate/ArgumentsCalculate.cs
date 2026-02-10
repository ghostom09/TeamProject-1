using UnityEngine;

public class ArgumentsCalculate
{
    private ItemData itemData;
    public void CalculateArguments(ItemData item)
    {
        if (item == null) return;
        itemData = item;
        switch (item.itemType)
        {
            case ItemType.Skill:
                Skill();
                break;
            case ItemType.Stat:
                Stat();
                break;
        }
    }

    void Stat()
    {
        switch (itemData.statType)
        {
            case StatType.Health:
                break;
            case StatType.Attack:
                break;
            case StatType.Speed:
                break;
            case StatType.Range:
                break;
            case StatType.AttackSpeed:
                break;
        }
    }

    void Skill()
    {
        switch (itemData.skillType)
        {
            case SkillType.GunShot:
                break;
            case SkillType.GunTrigger:
                break;
            case SkillType.GunUltra:
                break;
            case SkillType.SwordMove:
                break;
            case SkillType.SwordUltra:
                break;
            case SkillType.SwordCC:
                break;
        }
    }
}
