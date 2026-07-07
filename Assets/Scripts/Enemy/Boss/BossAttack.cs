using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour, IBossReset
{
    private static readonly int NormalAttackHash = Animator.StringToHash("NormalAttack");
    private static readonly int ShortSkillHash = Animator.StringToHash("ShortSkill");
    private static readonly int LongSkillHash = Animator.StringToHash("LongSkill");
    private static readonly int UltimateHash = Animator.StringToHash("Ultimate");
    private static readonly int IdleStateHash = Animator.StringToHash("Idle");

    [SerializeField] private Animator animator;

    private GameObject target;
    private BossStats bossStats;
    
    private BossMove bossMove;
    private BossHit bossHit;
    private LineRenderer lineRenderer;
    
    private bool isAttacking = false;

    private BossSkills normalAttack;
    private BossSkills shortSkill;
    private BossSkills longSkill;
    private BossSkills passiveSkill;
    private BossSkills ultimateSkill;
    
    private Dictionary<BossSkillType, IBossSkillStrategy> strategies = 
        new Dictionary<BossSkillType, IBossSkillStrategy>();
    private readonly List<GameObject> spawnedSkillObjects = new();

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
    
    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject trakingProjectilePrefab;
    [SerializeField] private GameObject ultimateProjectilePrefab;

    [Header("Magic Circle Prefabs")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject trackingPrefab;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private GameObject normalPrefab;

    [Header("Warrior Attack Effect Prefabs")]
    [SerializeField] private GameObject warriorNormalEffectPrefab;
    [SerializeField] private GameObject warriorShortEffectPrefab;
    [SerializeField] private GameObject warriorLongEffectPrefab;
    [SerializeField] private GameObject warriorUltimateEffectPrefab;
    
    public bool isUltimate;
    private bool usedUltimate;
    private bool hasNormalAttackParam;
    private bool hasShortSkillParam;
    private bool hasLongSkillParam;
    private bool hasUltimateParam;

    private void Awake()
    {
        bossMove = GetComponent<BossMove>();
        bossHit = GetComponent<BossHit>();
        lineRenderer = GetComponent<LineRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        DisableLineRenderer();
        CacheAnimatorParameters();
    }

    private void OnDisable()
    {
        CleanupSpawnedSkillObjects();
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        CleanupSpawnedSkillObjects();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CacheAnimatorParameters();

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

    public void CleanupSpawnedSkillObjects()
    {
        for (int i = spawnedSkillObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedSkillObjects[i] != null)
                Destroy(spawnedSkillObjects[i]);
        }

        spawnedSkillObjects.Clear();
        DisableLineRenderer();
    }

    private void RegisterSpawnedSkillObject(GameObject obj)
    {
        if (obj == null)
            return;

        spawnedSkillObjects.RemoveAll(spawnedObject => spawnedObject == null);
        spawnedSkillObjects.Add(obj);
    }

    private void DisableLineRenderer()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
            return;

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
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
        {
            TryUltimateSkill();
            return;
        }
        
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
        if (bossHit != null && bossHit.IsDead)
            return;

        isAttacking = false;
        bossMove.SetMoveLock(false);
        RestoreWarriorLocomotionState();
    }

    private void RestoreWarriorLocomotionState()
    {
        if (bossStats == null || bossStats.bossType != BossType.warrior || animator == null)
            return;

        animator.Play(IdleStateHash, 0, 0f);
        animator.Update(0f);
    }
    
    private void TryNormalAttack()
    {
        StartAttacking();
        SetWarriorTrigger(NormalAttackHash, hasNormalAttackParam);
        
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
        SetWarriorTrigger(ShortSkillHash, hasShortSkillParam);
        
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
        SetWarriorTrigger(LongSkillHash, hasLongSkillParam);
        
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
        if (projectilePrefab == null)
        {
            Debug.LogError("BossAttack projectilePrefab is not assigned.", this);
            return;
        }

        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        BossProjectile projectile = obj.GetComponent<BossProjectile>();

        if (projectile == null)
        {
            Debug.LogError($"{projectilePrefab.name} does not have a BossProjectile component.", projectilePrefab);
            Destroy(obj);
            return;
        }
        
        projectile.Init(dmg, range, dir, target, BossType.magician);
        RegisterSpawnedSkillObject(obj);
    }

    public GameObject SpawnMagicCircle(BossSkillType skillType, Vector2 position, float scale, float duration)
    {
        GameObject magicCirclePrefab = GetMagicCirclePrefab(skillType);

        if (magicCirclePrefab == null)
            return null;

        GameObject obj = Instantiate(magicCirclePrefab, position, Quaternion.identity);
        obj.transform.localScale = Vector3.one * scale;
        RegisterSpawnedSkillObject(obj);

        Destroy(obj, duration > 0f ? duration : 3f);

        return obj;
    }

    private GameObject GetMagicCirclePrefab(BossSkillType skillType)
    {
        switch (skillType)
        {
            case BossSkillType.Normal:
                return normalPrefab;
            case BossSkillType.shortDistance:
                return trackingPrefab;
            case BossSkillType.longDistance:
                return laserPrefab;
            case BossSkillType.passive:
                return spawnPrefab;
            default:
                return null;
        }
    }

    public GameObject SpawnWarriorAttackEffect(
        BossSkillType skillType,
        Vector2 position,
        Vector2 direction,
        float scale,
        float duration)
    {
        GameObject effectPrefab = GetWarriorAttackEffectPrefab(skillType);

        if (effectPrefab == null)
            return null;

        float angle = direction.sqrMagnitude > 0.001f
            ? Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
            : 0f;

        GameObject obj = Instantiate(effectPrefab, position, Quaternion.Euler(0f, 0f, angle));
        obj.transform.localScale = Vector3.one * Mathf.Max(0.1f, scale);
        RegisterSpawnedSkillObject(obj);
        Destroy(obj, duration > 0f ? duration : 1f);

        return obj;
    }

    private GameObject GetWarriorAttackEffectPrefab(BossSkillType skillType)
    {
        switch (skillType)
        {
            case BossSkillType.Normal:
                return warriorNormalEffectPrefab;
            case BossSkillType.shortDistance:
                return warriorShortEffectPrefab;
            case BossSkillType.longDistance:
                return warriorLongEffectPrefab;
            case BossSkillType.ultimate:
                return warriorUltimateEffectPrefab != null ? warriorUltimateEffectPrefab : warriorLongEffectPrefab;
            default:
                return null;
        }
    }
    
    public void ShootTrakingProjectile(Vector2 dir, float dmg, float range, Vector2 starting)
    {
        if (trakingProjectilePrefab == null)
        {
            Debug.LogError("BossAttack trakingProjectilePrefab is not assigned.", this);
            return;
        }

        GameObject obj = Instantiate(trakingProjectilePrefab, transform.position, Quaternion.identity);
        TrackingProjectile projectile = obj.GetComponent<TrackingProjectile>();

        if (projectile == null)
        {
            Debug.LogError($"{trakingProjectilePrefab.name} does not have a TrackingProjectile component.", trakingProjectilePrefab);
            Destroy(obj);
            return;
        }
        
        projectile.Init( dmg, range, target, starting);
        RegisterSpawnedSkillObject(obj);
    }
    
    public void ShootUltimateProjectile(Vector2 dir, float dmg, float range)
    {
        if (ultimateProjectilePrefab == null)
        {
            Debug.LogError("BossAttack ultimateProjectilePrefab is not assigned.", this);
            return;
        }

        GameObject obj = Instantiate(ultimateProjectilePrefab, transform.position, Quaternion.identity);
        BossProjectile projectile = obj.GetComponent<BossProjectile>();

        if (projectile == null)
        {
            Debug.LogError($"{ultimateProjectilePrefab.name} does not have a BossProjectile component.", ultimateProjectilePrefab);
            Destroy(obj);
            return;
        }
        
        projectile.Init(dmg, range, dir, target, BossType.warrior);
        RegisterSpawnedSkillObject(obj);
    }
    
    public void TryUltimateSkill()
    {
        StartAttacking();
        SetWarriorTrigger(LongSkillHash, hasLongSkillParam);
        
        strategies[BossSkillType.ultimate]?.TryAttack(gameObject, target, dir, () => 
        {
            nextSkillTime = timer + skillInterval;
            usedUltimate = true;
            StopAttacking();
        });
    }

    private void CacheAnimatorParameters()
    {
        hasNormalAttackParam = HasAnimatorParameter(NormalAttackHash);
        hasShortSkillParam = HasAnimatorParameter(ShortSkillHash);
        hasLongSkillParam = HasAnimatorParameter(LongSkillHash);
        hasUltimateParam = HasAnimatorParameter(UltimateHash);
    }

    private void SetWarriorTrigger(int parameterHash, bool hasParameter)
    {
        if (bossStats == null || bossStats.bossType != BossType.warrior)
            return;

        if (animator != null && hasParameter)
            animator.SetTrigger(parameterHash);
    }

    private bool HasAnimatorParameter(int parameterHash)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash)
                return true;
        }

        return false;
    }
}

