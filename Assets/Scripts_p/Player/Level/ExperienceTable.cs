using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ExperienceTable")]
public class ExperienceTable : ScriptableObject
{
    public List<int> requiredExp;

    public int GetRequiredExp(int level)
    {
        if (level - 1 < requiredExp.Count)
            return requiredExp[level - 1];

        return requiredExp[^1];
    }
}
