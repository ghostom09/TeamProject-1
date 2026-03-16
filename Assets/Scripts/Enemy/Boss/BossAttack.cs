using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour, IBossReset
{
    private GameObject target;

    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject hitArea;
    
    private bool isAttacking = false;

    private BossSkills normalAttack;
    private BossSkills shortSkill;
    private BossSkills longSkill;
    
    private Dictionary<BossSkillType, IBossSkillStrategy> strategies = 
        new Dictionary<BossSkillType, IBossSkillStrategy>();

    private float normalCooldown;
    private float shortCooldown;
    private float passiveCooldown;
    private float longCooldown;
    
    private float shortRange;
    private float longRange;
    
    private float nextSkillTime;
    private float skillInterval;
    
    private Vector2 dir;
    private float distance;
    
    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        this.target = target;
        
        strategies.Clear();

        switch (stats.bossType)
        {
            case BossType.warrior:

                strategies.Add(BossSkillType.Normal, new WarriorNormalAttack());
                strategies.Add(BossSkillType.shortDistance, new WarriorShortAttack());
                strategies.Add(BossSkillType.longDistance, new WarriorLongAttack());
                strategies.Add(BossSkillType.passive, new WarriorPassiveAttack());
                strategies.Add(BossSkillType.ultimate, new WarriorUltimateAttack());

                break;

            case BossType.magician:

                strategies.Add(BossSkillType.Normal, new MagicianNormalAttack());
                strategies.Add(BossSkillType.shortDistance, new MagicianShortAttack());
                strategies.Add(BossSkillType.longDistance, new MagicianLongAttack());
                strategies.Add(BossSkillType.passive, new MagicianPassiveAttack());
                strategies.Add(BossSkillType.ultimate, new MagicianUltimateAttack());

                break;
        }

        foreach (var skill in stats.skills)
        {
            switch (skill.skillType)
            {
                case BossSkillType.Normal:
                    normalAttack = skill;
                    normalCooldown = skill.cooldown;
                    strategies[BossSkillType.Normal]?.
                        Init(skill, this, hitBox, hitArea);
                    break;
                
                case BossSkillType.shortDistance:
                    shortSkill = skill;
                    shortCooldown = skill.cooldown;
                    shortRange = skill.attackRange;
                    strategies[BossSkillType.shortDistance]?.
                        Init(skill, this, hitBox, hitArea);
                    break;

                case BossSkillType.longDistance:
                    longSkill = skill;
                    longCooldown = skill.cooldown;
                    longRange = skill.attackRange;
                    strategies[BossSkillType.longDistance]?.
                        Init(skill, this, hitBox, hitArea);
                    break;
                
                case BossSkillType.passive:
                    shortSkill = skill;
                    passiveCooldown = skill.cooldown;
                    strategies[BossSkillType.shortDistance]?.
                        Init(skill, this, hitBox, hitArea);
                    break;
                
                case BossSkillType.ultimate:
                    shortSkill = skill;
                    strategies[BossSkillType.shortDistance]?.
                        Init(skill, this, hitBox, hitArea);
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
        
        distance = ((target.transform.position.x - transform.position.x) *
                    (target.transform.position.x - transform.position.x)) +
                   ((target.transform.position.y - transform.position.y) *
                    (target.transform.position.y - transform.position.y));
        
        dir = (target.transform.position - transform.position).normalized;

        ChooseSkill();
    }
    
    private void ChooseSkill()
    {
        if (isAttacking) return;
        
        bool used = false;

        if (Time.time >= nextSkillTime)
        {
            if (distance <= shortSkill.attackRange * shortSkill.attackRange && Time.time >= shortCooldown)
            {
                TryShortSkill();
                return;
            }
            else if (distance <= longSkill.attackRange * longSkill.attackRange && Time.time >= longCooldown)
            {
                TryLongSkill();
                return;
            }
        }

        if (Time.time >= normalCooldown)
        {
            TryNormalAttack();
        }
    }
    
    private void TryNormalAttack()
    {
        isAttacking = true;
        
        strategies[BossSkillType.Normal]?.TryAttack(gameObject, target, dir, () => 
        {
            normalCooldown = Time.time + normalAttack.cooldown;
            isAttacking = false;
        });
    }
    
    private void TryShortSkill()
    {
        isAttacking = true;
        strategies[BossSkillType.shortDistance]?.TryAttack(gameObject, target, dir, () => 
        {
            shortCooldown = Time.time + shortSkill.cooldown;
            nextSkillTime = Time.time + skillInterval;
            isAttacking = false;
        });
    }

    private void TryLongSkill()
    {
        isAttacking = true;
        strategies[BossSkillType.longDistance]?.TryAttack(gameObject, target, dir, () => 
        {
            longCooldown = Time.time + longSkill.cooldown;
            nextSkillTime = Time.time + skillInterval;
            isAttacking = false;
        });
    }
}
