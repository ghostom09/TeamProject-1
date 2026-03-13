using UnityEngine;
using System.Collections.Generic;

public class ArgumentDataManager : MonoBehaviour
{
    public static ArgumentDataManager Instance;

    [SerializeField] private List<SkillArgumentData> skillArguments = new();
    [SerializeField] private List<StatArgumentData> statArguments = new();

    private ArgumentManager argumentManager;

    private jobType nowJob;

    private List<ArgumentData> arguments = new();
    private List<ArgumentData> nowArguments = new();

    private Dictionary<SkillType, OwnedSkill> ownedSkills = new();

    void Awake()
    {
        Instance = this;

        argumentManager = GetComponent<ArgumentManager>();

        SetArguments();
    }

    public void GetJob(jobType job)
    {
        nowJob = job;
    }

    private void SetArguments()
    {
        arguments.Clear();

        arguments.AddRange(skillArguments);
        arguments.AddRange(statArguments);
    }

    private ArgumentData RandomType(List<ArgumentData> randomBox)
    {
        if (randomBox.Count == 0)
            return null;

        int totalWeight = 0;

        foreach (var item in randomBox)
        {
            totalWeight += item.weight;
        }

        int randomValue = Random.Range(0, totalWeight);

        int currentWeight = 0;

        foreach (var item in randomBox)
        {
            currentWeight += item.weight;

            if (randomValue < currentWeight)
                return item;
        }

        return randomBox[0];
    }

    public List<ArgumentData> GetRandomArguments(int count)
    {
        nowArguments.Clear();

        List<ArgumentData> pool = new();

        foreach (var arg in arguments)
        {
            if (!arg.IsAllowed(nowJob))
                continue;

            if (arg is SkillArgumentData skillData)
            {
                if (!CanSkillAppear(skillData))
                    continue;
            }

            pool.Add(arg);
        }

        int spawn = Mathf.Min(count, pool.Count);

        List<ArgumentData> results = new();

        for (int i = 0; i < spawn; i++)
        {
            var item = RandomType(pool);

            if (item == null)
                break;

            nowArguments.Add(item);
            results.Add(item);

            pool.Remove(item);
        }

        return results;
    }

    public ArgumentData MakeArgument(int id)
    {
        if (id < 0 || id >= nowArguments.Count)
            return null;

        return nowArguments[id];
    }

    public ArgumentResult ConvertToResult(ArgumentData data)
    {
        ArgumentResult result = new ArgumentResult();

        if (data is StatArgumentData stat)
        {
            result.kind = ArgumentKind.Stat;
            result.statType = stat.statType;
            result.statValue = stat.value;
        }
        else if (data is SkillArgumentData skill)
        {
            result.kind = ArgumentKind.Skill;
            result.skillType = skill.skillType;
            result.skillLevel = skill.level;
        }

        Debug.Log(result);
        return result;
    }

    public void ApplySkillResult(SkillArgumentData skillData)
    {
        if (ownedSkills.TryGetValue(skillData.skillType, out var owned))
        {
            owned.Upgrade();
        }
        else
        {
            OwnedSkill newSkill = new OwnedSkill
            {
                data = skillData,
                currentLevel = 0
            };

            newSkill.Upgrade();
            ownedSkills.Add(skillData.skillType, newSkill);
        }
    }

    private bool CanSkillAppear(SkillArgumentData skillData)
    {
        if (ownedSkills.TryGetValue(skillData.skillType, out var owned))
        {
            return owned.CanUpgrade();
        }

        return true;
    }
    
    public int GetSkillLevel(SkillArgumentData data)
    {
        if (ownedSkills.TryGetValue(data.skillType, out var owned))
            return owned.currentLevel;

        return 0;
    }
}