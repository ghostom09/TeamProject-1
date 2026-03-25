using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour, IBossReset
{
    private GameObject target;
    private BossStats bossStats;
    
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
    private float timer;
    
    private BossSkillExtraCondition conditioner = new BossSkillExtraCondition();
    
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject trakingProjectilePrefab;
    [SerializeField] private GameObject ultimateProjectilePrefab;
    
    public bool isUltimate;
    private bool usedUltimate;

    private void Awake()
    {
        bossMove = GetComponent<BossMove>();
        bossHit = GetComponent<BossHit>();
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        this.target = target;
        bossStats = stats;
        isUltimate = false;
        usedUltimate = false;
        isAttacking = false;
        timer = 0;
        
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
                        Init(gameObject, skill, this, target);
                    break;
                
                case BossSkillType.shortDistance:
                    shortSkill = skill;
                    shortCooldown = skill.cooldown;
                    shortRange = skill.attackRange;
                    strategies[BossSkillType.shortDistance]?.
                        Init(gameObject, skill, this, target);
                    break;

                case BossSkillType.longDistance:
                    longSkill = skill;
                    longCooldown = skill.cooldown;
                    longRange = skill.attackRange;
                    strategies[BossSkillType.longDistance]?.
                        Init(gameObject, skill, this, target);
                    break;
                
                case BossSkillType.passive:
                    passiveSkill = skill;
                    passiveCooldown = skill.cooldown;
                    strategies[BossSkillType.passive]?.
                        Init(gameObject, skill, this, target);
                    break;
                
                case BossSkillType.ultimate:
                    ultimateSkill = skill;
                    strategies[BossSkillType.ultimate]?.
                        Init(gameObject, skill, this, target);
                    break;
            }
        }
        
        skillInterval = stats.skillInterval;
        nextSkillTime = 0f;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
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
        
        if (isUltimate && !usedUltimate)
            TryUltimateSkill();
        
        bool used = false;

        if (timer >= nextSkillTime)
        {
            if (distance <= shortSkill.attackRange * shortSkill.attackRange &&
                conditioner.CanUse(bossStats.bossType, BossSkillType.shortDistance, gameObject, target) &&
                timer >= shortCooldown)
            {
                TryShortSkill();
                return;
            }
            if (distance <= longSkill.attackRange * longSkill.attackRange &&
                conditioner.CanUse(bossStats.bossType, BossSkillType.longDistance, gameObject, target) &&
                timer >= longCooldown)
            {
                TryLongSkill();
                return;
            }
        }
        
        if (timer >= passiveCooldown)
        {
            TryPassiveSkill();
        }
        
        if (distance <= normalAttack.attackRange * normalAttack.attackRange && timer >= normalCooldown)
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
            normalCooldown = timer + normalAttack.cooldown;
            nextSkillTime = timer + skillInterval;
            StopAttacking();
        });
    }
    
    private void TryShortSkill()
    {
        StartAttacking();
        
        strategies[BossSkillType.shortDistance]?.TryAttack(gameObject, target, dir, () => 
        {
            shortCooldown = timer + shortSkill.cooldown;
            nextSkillTime = timer + skillInterval;
            StopAttacking();
        });
    }

    private void TryLongSkill()
    {
        StartAttacking();
        
        strategies[BossSkillType.longDistance]?.TryAttack(gameObject, target, dir, () => 
        {
            longCooldown = timer + longSkill.cooldown;
            nextSkillTime = timer + skillInterval;
            StopAttacking();
        });
    }

    private void TryPassiveSkill()
    {
        StartAttacking();
        
        strategies[BossSkillType.passive]?.TryAttack(gameObject, target, dir, () => 
        {
            passiveCooldown = timer + passiveSkill.cooldown;
            nextSkillTime = timer + skillInterval;
            StopAttacking();
        });
    }

    public void Shield(int shieldStock)
    {
        bossHit.MakeShield(shieldStock);
    }

    public void ShootProjectile(Vector2 dir, float dmg, float range)
    {
        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        BossProjectile projectile = obj.GetComponent<BossProjectile>();
        
        projectile.Init(dmg, range, dir, target, BossType.magician);
    }
    
    public void ShootTrakingProjectile(Vector2 dir, float dmg, float range, Vector2 starting)
    {
        GameObject obj = Instantiate(trakingProjectilePrefab, transform.position, Quaternion.identity);
        TrackingProjectile projectile = obj.GetComponent<TrackingProjectile>();
        
        projectile.Init( dmg, range, target, starting);
    }
    
    public void ShootUltimateProjectile(Vector2 dir, float dmg, float range)
    {
        GameObject obj = Instantiate(ultimateProjectilePrefab, transform.position, Quaternion.identity);
        BossProjectile projectile = obj.GetComponent<BossProjectile>();
        
        projectile.Init(dmg, range, dir, target, BossType.warrior);
    }
    
    public void TryUltimateSkill()
    {
        StartAttacking();
        
        strategies[BossSkillType.ultimate]?.TryAttack(gameObject, target, dir, () => 
        {
            nextSkillTime = timer + skillInterval;
            usedUltimate = true;
            StopAttacking();
        });
    }
}

