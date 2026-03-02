using System;
using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField]private ArgumentManager argumentManager;
    [SerializeField] private List<SkillArgumentData> allSkills = new List<SkillArgumentData>();
    [SerializeField] private List<StatArgumentData>  allStats  = new List<StatArgumentData>();

    private Dictionary<JobType, List<SkillArgumentData>> skills = new ();
    
    // [SerializeField] private PlayerStats playerStats;
    private int level; //플레이어에서 레벨 갖고 오기
    private JobType nowJob;
    private void Awake()
    {
        InitializeOwnedData();
    }
    
    private void InitializeOwnedData()
    {
        skills.Clear();
        nowJob = JobType.Sword;
        foreach (var skillData in allSkills)
        {
            JobType job = skillData.jobType;

            if (!skills.ContainsKey(job))
            {
                skills[job] = new List<SkillArgumentData>();
            }
            skills[job].Add(skillData);
        }
    }

    public bool RandomType()
    {
        return UnityEngine.Random.value < 0.8f;
    }

    public StatArgumentData RandomStat()
    {
        int index = UnityEngine.Random.Range(0, allStats.Count);
        return allStats[index];
    }
    public SkillArgumentData RandomSkill()
    {
        List<SkillArgumentData> jobSkills = skills[nowJob];
        int index = UnityEngine.Random.Range(0, jobSkills.Count);

        return jobSkills[index];
    }
}
