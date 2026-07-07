using System;
using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour, IDamageable, IEnemyReset, IHitEffectReceiver
{
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int ReviveHash = Animator.StringToHash("Revive");

    [SerializeField] private ParticleSystem gunUltraHitEffect;
    [SerializeField] private float gunUltraEffectOffset = 0.8f;
    [SerializeField] private Animator animator;
    [SerializeField] private float deathAnimationDelay = 0.3f;

    private SpriteRenderer renderer;
    private EnemyMove enemyMove;
    private EnemySpawnerManager spawnerManager;
    private PlayerLevelManager levelManager;
    private Color color;
    
    public float health;
    public float maxHealth;
    private int exp;
    
    private Coroutine slowRoutine;
    private Coroutine knockRoutine;
    private float _x = 1;
    private int count;
    private float moveTimer = 0f;

    private bool isKnocked;
    private float knockMultiplier = 1f;
    private bool hasHitParam;
    private bool hasDieParam;
    private bool hasReviveParam;
    private bool isDead;
    private Collider2D[] colliders;
    
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<SpriteRenderer>();
        enemyMove = GetComponent<EnemyMove>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        colliders = GetComponents<Collider2D>();
        CacheAnimatorParameters();
        CacheGunUltraHitEffect();
        StopChildParticles();
    }

    private void OnEnable()
    {
        isDead = false;
        SetCollidersEnabled(true);
        SetTrigger(ReviveHash, hasReviveParam);
        StopChildParticles();
    }

    public void SetAnimator(Animator targetAnimator)
    {
        animator = targetAnimator;
        CacheAnimatorParameters();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CacheAnimatorParameters();

        health = stats.health;
        maxHealth = stats.health;
        exp = stats.exp;
        spawnerManager = m;
        color = renderer.color;
        levelManager = target.GetComponent<PlayerLevelManager>();
        isDead = false;
        SetCollidersEnabled(true);
        SetTrigger(ReviveHash, hasReviveParam);
    }

    public void TakeDamage(float dmg)
    {
        if (isDead)
            return;

        health -= dmg;
        SetTrigger(HitHash, hasHitParam);
        StartCoroutine(Hit());
        
        if (health <= 0)
        {
            Die();
        }
    }

    public void ApplySlow(float slowPercent, float slowDuration)
    {
        if(!gameObject.activeInHierarchy)
            return;
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(Slow(slowPercent, slowDuration));
    }

    private IEnumerator Slow(float percent, float duration)
    {
        yield return new WaitForSeconds(duration);
        slowRoutine = null;
    }

    public void ApplyKnockback(Vector2 dir, float power, float duration){enemyMove.ApplyKnockback(dir, power, duration);}

    public void PlayGunUltraNormalHitEffect(Vector2 attackerPosition, Vector2 hitDirection)
    {
        CacheGunUltraHitEffect();

        Vector2 effectDirection = hitDirection.sqrMagnitude > 0f
            ? hitDirection.normalized
            : ((Vector2)transform.position - attackerPosition).normalized;

        if (effectDirection.sqrMagnitude <= 0f)
            effectDirection = Vector2.right;

        Vector3 effectPosition = transform.position + (Vector3)(effectDirection * gunUltraEffectOffset);
        Quaternion effectRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(effectDirection.y, effectDirection.x) * Mathf.Rad2Deg);

        ParticleSystem effect = gunUltraHitEffect != null
            ? Instantiate(gunUltraHitEffect, effectPosition, effectRotation)
            : CreateDefaultGunUltraHitEffect(effectPosition, effectRotation);

        effect.gameObject.SetActive(true);
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }

    private void CacheGunUltraHitEffect()
    {
        if (gunUltraHitEffect != null)
            return;

        Transform effectTransform = transform.Find("Particle(UltraSpark)");
        if (effectTransform != null)
            gunUltraHitEffect = effectTransform.GetComponent<ParticleSystem>();
    }

    private void StopChildParticles()
    {
        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>(true))
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private ParticleSystem CreateDefaultGunUltraHitEffect(Vector3 position, Quaternion rotation)
    {
        GameObject effectObject = new GameObject("GunUltraHitEffect");
        effectObject.transform.position = position;
        effectObject.transform.rotation = rotation;

        ParticleSystem effect = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = effect.main;
        main.duration = 0.25f;
        main.startLifetime = 0.25f;
        main.startSpeed = 5f;
        main.startSize = 0.18f;
        main.startColor = new Color(1f, 0.85f, 0.15f, 1f);
        main.loop = false;

        ParticleSystem.EmissionModule emission = effect.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)12)
        });

        ParticleSystem.ShapeModule shape = effect.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.05f;

        ParticleSystemRenderer effectRenderer = effect.GetComponent<ParticleSystemRenderer>();
        effectRenderer.sortingOrder = 10;

        return effect;
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
        renderer.color = color;
    }
    
    private void Die()
    {
        isDead = true;
        GameResultTracker.Instance?.RegisterKill();
        SetTrigger(DieHash, hasDieParam);
        StopAllCoroutines();
        levelManager.AddExp(exp);

        SetCollidersEnabled(false);
        enemyMove.SetMoveLock(true);

        if (hasDieParam && deathAnimationDelay > 0f)
        {
            StartCoroutine(ReturnToPoolAfterDeathAnimation());
            return;
        }

        ReturnToPool();
    }

    private IEnumerator ReturnToPoolAfterDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDelay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);

        spawnerManager.ReturnToPool(GetComponent<Enemy>());
    }

    private void CacheAnimatorParameters()
    {
        hasHitParam = false;
        hasDieParam = false;
        hasReviveParam = false;

        if (animator == null)
            return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == HitHash)
                hasHitParam = true;
            else if (parameter.nameHash == DieHash)
                hasDieParam = true;
            else if (parameter.nameHash == ReviveHash)
                hasReviveParam = true;
        }
    }

    private void SetTrigger(int parameterHash, bool hasParameter)
    {
        if (animator != null && hasParameter)
            animator.SetTrigger(parameterHash);
    }

    private void SetCollidersEnabled(bool isEnabled)
    {
        if (colliders == null)
            return;

        foreach (Collider2D col in colliders)
        {
            col.enabled = isEnabled;
        }
    }
}
