using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHit : MonoBehaviour, IBossReset, IDamageable
{
    private static readonly int DieHash = Animator.StringToHash("Die");

    [SerializeField] private float health;
    [SerializeField] private Animator animator;
    [SerializeField] private float deathAnimationDelay = 0.5f;
    private float maxHealth;
    private SpriteRenderer renderer;
    private Color color;
    private int shield;
    [SerializeField] private float reductionRate;
    [SerializeField] private GameObject Shield;
    
    private BossMove _bossMove;
    private BossAttack _bossAttack;

    private int exp;

    private PlayerLevelManager levelManager;
    private PlayerInput playerInput;
    
    private EnemySpawnerManager spawnerManager;
    
    [SerializeField] private RectTransform healthBar;
    private float maxHealthBar;
    private bool hasDieParam;
    private bool isDead;
    public bool IsDead => isDead;

    private void Awake()
    {
        _bossMove = GetComponent<BossMove>();
        _bossAttack = GetComponent<BossAttack>();
        renderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CacheAnimatorParameters();
        maxHealthBar = healthBar.sizeDelta.x;
        color = renderer.color;
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CacheAnimatorParameters();
        renderer.color = color;
        health = stats.health;
        maxHealth = stats.health;
        shield = 0;
        isDead = false;
        exp = stats.exp;
        spawnerManager = m;
        healthBar.sizeDelta = new Vector2(maxHealthBar, healthBar.sizeDelta.y);
        if (Shield != null)
            Shield.SetActive(false);

        levelManager = target.GetComponent<PlayerLevelManager>();
        playerInput = FindPlayerInput(target);
    }

    public void TakeDamage(float dmg)
    {
        if (isDead)
            return;

        if (shield > 0)
        {
            shield--;
            dmg *= 1-reductionRate;
            if(shield <= 0 && Shield != null)
                Shield.SetActive(false);
        }
        health -= dmg;
        StartCoroutine(Hit());
        
        healthBar.sizeDelta = new Vector2(health / maxHealth * maxHealthBar, healthBar.sizeDelta.y);
        
        if(health <= maxHealth * 0.33f)
            _bossAttack.isUltimate = true;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        StopBossCoroutines();
        _bossAttack?.CleanupSpawnedSkillObjects();
        playerInput?.DeactivateInputShuffle();
        renderer.color = color;

        if (animator != null && hasDieParam)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
            animator.SetTrigger(DieHash);
        }

        levelManager.AddExp(exp);

        if (hasDieParam && deathAnimationDelay > 0f)
        {
            StartCoroutine(BossDieAfterAnimation());
            return;
        }

        spawnerManager.BossDie();
    }

    private void StopBossCoroutines()
    {
        foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
        {
            behaviour.StopAllCoroutines();
        }
    }

    private IEnumerator BossDieAfterAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDelay);
        spawnerManager.BossDie();
    }
    
    private IEnumerator Hit()
    {
        float time = 0;
        while (time < 0.5f)
        {
            renderer.color = Color.red;
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = Color.white;
    }
    
    public void MakeShield(int shieldStock)
    {
        if (Shield != null)
            Shield.SetActive(true);
        shield = shieldStock;
    }
    
    public void ApplySlow(float slowPercent, float slowDuration) { }
    
    public void ApplyKnockback(Vector2 dir, float power, float duration) {_bossMove.ApplyKnockback(dir, power, duration);}

    private PlayerInput FindPlayerInput(GameObject target)
    {
        if (target == null)
            return null;

        PlayerInput input = target.GetComponent<PlayerInput>();
        if (input != null)
            return input;

        input = target.GetComponentInParent<PlayerInput>();
        if (input != null)
            return input;

        return target.GetComponentInChildren<PlayerInput>();
    }

    private void CacheAnimatorParameters()
    {
        hasDieParam = HasAnimatorParameter(DieHash);
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
