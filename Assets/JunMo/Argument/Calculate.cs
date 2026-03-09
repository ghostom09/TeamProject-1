using UnityEngine;

[System.Serializable]
public class OwnedSkill
{
    public SkillArgumentData data;
    public int currentLevel = 0;

    public float GetCurrentValue() // 스킬누르면 호출, 레벨에 맞게 값 반환
    {
        int idx = Mathf.Clamp(currentLevel - 1, 0, data.levelValues.Length - 1);
        return data.levelValues[idx];
    }

    public bool CanUpgrade() => currentLevel < data.maxLevel;
    public void Upgrade() 
    { 
        if (CanUpgrade()) currentLevel++; 
    }
}

[System.Serializable]
public class OwnedStat
{
    public StatArgumentData data;
    public int count = 0;

    public float GetModifierValue() => data.value * count; // ex) playerstats = health + GetModifierValue 이런씩
}