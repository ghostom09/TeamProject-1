using System;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillExtraCondition
{
    private readonly Dictionary<(BossType, BossSkillType), Func<GameObject, GameObject, bool>> conditionMap;

    public BossSkillExtraCondition()
    {
        conditionMap = new Dictionary<(BossType, BossSkillType), Func<GameObject, GameObject, bool>>();

        conditionMap.Add((BossType.warrior, BossSkillType.shortDistance), (boss, target) => true);
    }

    public bool CanUse(BossType bossType, BossSkillType skillType, GameObject boss, GameObject target)
    {
        var key = (bossType, skillType);
        
        if (conditionMap.TryGetValue(key, out var condition))
        {
            return condition(boss, target);
        }

        return true;
    }
}
