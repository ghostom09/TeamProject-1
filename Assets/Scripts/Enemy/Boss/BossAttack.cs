using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour, IBossReset
{
    private GameObject target;
    
    [SerializeField] private GameObject hitArea;
    
    private BossMove bossMove;
    private BossHit bossHit;
    
    private bool isAttacking = false;

    private BossSkills normalAttack;
    private BossSkills shortSkill;
    private BossSkills longSkill;
    private BossSkills passiveSkill;
    private BossSkills ultimateSkill;
    
    private Dictionary<BossSkillType, IBossSkillStrategy> strategies = 
        new Dictionary<BossSkillType, IBossSkillStrategy>();

    private float normalCooldown;
    private float shortCooldown;
    private float longCooldown;
    private float passiveCooldown;
    
    private float shortRange;
    private float longRange;
    
    private float nextSkillTime;
    private float skillInterval;
    
    private Vector2 dir;
    private float distance;

    private void Awake()
    {
        bossMove = GetComponent<BossMove>();
        bossHit = GetComponent<BossHit>();
    }

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
                        Init(gameObject, skill, this, hitArea, target);
                    break;
                
                case BossSkillType.shortDistance:
                    shortSkill = skill;
                    shortCooldown = skill.cooldown;
                    shortRange = skill.attackRange;
                    strategies[BossSkillType.shortDistance]?.
                        Init(gameObject, skill, this, hitArea, target);
                    break;

                case BossSkillType.longDistance:
                    longSkill = skill;
                    longCooldown = skill.cooldown;
                    longRange = skill.attackRange;
                    strategies[BossSkillType.longDistance]?.
                        Init(gameObject, skill, this, hitArea, target);
                    break;
                
                case BossSkillType.passive:
                    passiveSkill = skill;
                    passiveCooldown = skill.cooldown;
                    strategies[BossSkillType.passive]?.
                        Init(gameObject, skill, this, hitArea, target);
                    break;
                
                case BossSkillType.ultimate:
                    ultimateSkill = skill;
                    strategies[BossSkillType.ultimate]?.
                        Init(gameObject, skill, this, hitArea, target);
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
            if (distance <= shortSkill.attackRange * shortSkill.attackRange &&
                bossMove.isGrounded &&
                Time.time >= shortCooldown)
            {
                TryShortSkill();
                return;
            }
            if (shortSkill.attackRange * shortSkill.attackRange < distance && 
                distance <= longSkill.attackRange * longSkill.attackRange &&
                target.transform.position.y >= transform.position.y - 1 &&
                Time.time >= longCooldown)
            {
                TryLongSkill();
                return;
            }
        }
        
        if (Time.time >= passiveCooldown)
        {
            TryPassiveSkill();
        }
        
        if (distance <= normalAttack.attackRange * normalAttack.attackRange && Time.time >= normalCooldown)
        {
            TryNormalAttack();
        }
    }

    private void StartAttacking()
    {
        isAttacking = true;
        bossMove.SetMoveLock(true);
        
    }

    private void StopAttacking()
    {
        isAttacking = false;
        bossMove.SetMoveLock(false);
    }
    
    private void TryNormalAttack()
    {
        StartAttacking();
        
        strategies[BossSkillType.Normal]?.TryAttack(gameObject, target, dir, () => 
        {
            normalCooldown = Time.time + normalAttack.cooldown;
            nextSkillTime = Time.time + skillInterval;
            StopAttacking();
        });
    }
    
    private void TryShortSkill()
    {
        StartAttacking();
        
        strategies[BossSkillType.shortDistance]?.TryAttack(gameObject, target, dir, () => 
        {
            shortCooldown = Time.time + shortSkill.cooldown;
            nextSkillTime = Time.time + skillInterval;
            StopAttacking();
        });
    }

    private void TryLongSkill()
    {
        StartAttacking();
        
        Vector2 adaptiveDir = new Vector2(0.707f * bossMove.lookSide, 0.707f).normalized;; //45도
        
        strategies[BossSkillType.longDistance]?.TryAttack(gameObject, target, adaptiveDir, () => 
        {
            longCooldown = Time.time + longSkill.cooldown;
            nextSkillTime = Time.time + skillInterval;
            StopAttacking();
        });
    }

    private void TryPassiveSkill()
    {
        StartAttacking();
        
        strategies[BossSkillType.passive]?.TryAttack(gameObject, target, dir, () => 
        {
            passiveCooldown = Time.time + passiveSkill.cooldown;
            nextSkillTime = Time.time + skillInterval;
            StopAttacking();
        });
    }

    public void Shield(int shieldStock)
    {
        bossHit.MakeShield(shieldStock);
    }
    
    public void TryUltimateSkill()
    {
        
    }
}

