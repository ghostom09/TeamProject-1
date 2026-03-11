using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour, IBossReset
{
    private Transform target;

    private BossSkills normalAttack;
    private BossSkills shortSkill;
    private BossSkills middleSkill;
    private BossSkills longSkill;
    
    private Dictionary<BossSkillType, IBossSkillStrategy> strategies = 
        new Dictionary<BossSkillType, IBossSkillStrategy>();

    private float normalCooldown;
    private float shortCooldown;
    private float middleCooldown;
    private float longCooldown;
    
    private float shortRange;
    private float middleRange;
    private float longRange;
    
    private float nextSkillTime;
    private float skillInterval;
    
    private Vector2 dir;
    private float distance;
    
    private bool isCasting;
    
    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        this.target = target.transform;
        
        strategies.Clear();

        switch (stats.bossType)
        {
            case BossType.warrior:

                strategies.Add(BossSkillType.Normal, new WarriorNormalAttack());
                strategies.Add(BossSkillType.shortDistance, new WarriorShortAttack());
                strategies.Add(BossSkillType.middleDistance, new WarriorMiddleAttack());
                strategies.Add(BossSkillType.longDistance, new WarriorLongAttack());

                break;

            case BossType.magician:

                strategies.Add(BossSkillType.Normal, new MagicianNormalAttack());
                strategies.Add(BossSkillType.shortDistance, new MagicianShortAttack());
                strategies.Add(BossSkillType.middleDistance, new MagicianMiddleAttack());
                strategies.Add(BossSkillType.longDistance, new MagicianLongAttack());

                break;
        }

        foreach (var skill in stats.skills)
        {
            switch (skill.skills)
            {
                case BossSkillType.Normal:
                    normalAttack = skill;
                    normalCooldown = skill.cooldown;
                    strategies[BossSkillType.Normal]?.Init(skill, this);
                    break;
                
                case BossSkillType.shortDistance:
                    shortSkill = skill;
                    shortCooldown = skill.cooldown;
                    shortRange = skill.attackRange;
                    strategies[BossSkillType.shortDistance]?.Init(skill, this);
                    break;

                case BossSkillType.middleDistance:
                    middleSkill = skill;
                    middleCooldown = skill.cooldown;
                    middleRange = skill.attackRange;
                    strategies[BossSkillType.middleDistance]?.Init(skill, this);
                    break;

                case BossSkillType.longDistance:
                    longSkill = skill;
                    longCooldown = skill.cooldown;
                    longRange = skill.attackRange;
                    strategies[BossSkillType.longDistance]?.Init(skill, this);
                    break;
            }
        }
        
        skillInterval = stats.skillInterval;
        nextSkillTime = 0f;
    }
    
    private void Update()
    {
        if (target == null)
            return;
        
        if (isCasting)
            return;
        
        distance = ((target.transform.position.x - transform.position.x) *
                    (target.transform.position.x - transform.position.x)) +
                   ((target.transform.position.y - transform.position.y) *
                    (target.transform.position.y - transform.position.y));
        
        dir = (target.position - transform.position).normalized;

        ChooseSkill();
    }
    
    private void ChooseSkill()
    {
        bool used = false;

        if (Time.time >= nextSkillTime)
        {
            if (distance <= shortRange * shortRange && CanUse(shortCooldown))
            {
                TryShortSkill();
                used = true;
            }
            else if (distance <= middleRange * middleRange && CanUse(middleCooldown))
            {
                TryMiddleSkill();
                used = true;
            }
            else if (distance <= longRange * longRange && CanUse(longCooldown))
            {
                TryLongSkill();
                used = true;
            }
        }

        if (!used && CanUse(normalCooldown))
        {
            TryNormalAttack();
        }
    }
    
    private bool CanUse(float cooldown)
    {
        return Time.time >= cooldown;
    }
    
    public void EndCast()
    {
        isCasting = false;
        nextSkillTime = Time.time + skillInterval;
    }
    
    private void TryNormalAttack()
    {
        if (Time.time < normalCooldown)
            return;

        normalCooldown = Time.time + normalAttack.cooldown;

        isCasting = true;

        strategies[BossSkillType.Normal]?.TryAttack(gameObject, target, dir);
    }
    
    private void TryShortSkill()
    {
        Debug.Log($"단거리{distance}");
        if (Time.time < shortCooldown)
            return;

        shortCooldown = Time.time + shortSkill.cooldown;

        isCasting = true;

        strategies[BossSkillType.shortDistance]?.TryAttack(gameObject, target, dir);
    }

    private void TryMiddleSkill()
    {
        Debug.Log($"중거리{distance}");
        if (Time.time < middleCooldown)
            return;

        middleCooldown = Time.time + middleSkill.cooldown;

        isCasting = true;

        strategies[BossSkillType.middleDistance]?.TryAttack(gameObject, target, dir);
    }

    private void TryLongSkill()
    {
        Debug.Log($"장거리{distance}");
        if (Time.time < longCooldown)
            return;

        longCooldown = Time.time + longSkill.cooldown;

        isCasting = true;

        strategies[BossSkillType.longDistance]?.TryAttack(gameObject, target, dir);
    }
}
