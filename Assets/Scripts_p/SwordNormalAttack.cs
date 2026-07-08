using System.Collections.Generic;
using UnityEngine;

public class SwordNormalAttack : INormalAttack
{
    private const float EffectMinScale = 0.1f;
    private const float EffectRangeVisualScale = 0.88f;
    private int comboIndex = 0;
    private float lastAttackTime;
    private const float ComboResetTime = 1.5f;
    private const float EffectAngleOffset = -170f;

    private CharacterData data;
    private GameObject[] effectPrefabs;

    private LayerMask enemyLayer;

    public int LastAttackComboIndex { get; private set; }

    public void Init(CharacterData data)
    {
        this.data = data;
        enemyLayer = LayerMask.GetMask("Enemy", "Bullet");
    }

    public void SetEffectPrefabs(GameObject[] prefabs)
    {
        effectPrefabs = prefabs;
    }

    private float GetComboRangeMultiplier()
    {
        return comboIndex switch
        {
            0 => 0.8f,
            1 => 1.0f,
            2 => 1.2f,
            _ => 1f
        };
    }

    private float GetComboDamageMultiplier()
    {
        return comboIndex switch
        {
            0 => 0.5f,
            1 => 0.7f,
            2 => 1.2f,
            _ => 1f
        };
    }

    public bool TryAttack(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (Time.time > lastAttackTime + ComboResetTime)
            comboIndex = 0;

        float attackInterval = 1f / player.Stats.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
            return false;

        lastAttackTime = Time.time;
        LastAttackComboIndex = comboIndex;

        DoComboAttack(player, user, dir);

        return true;
    }

    private void DoComboAttack(Player player, GameObject user, Vector2 dir)
    {
        Vector2 origin = user.transform.position;

        float range = data.Range * GetComboRangeMultiplier();
        float damage = player.Stats.Damage * GetComboDamageMultiplier();
        float attackRadius = range * 0.5f;

        Vector2 center = origin + dir * attackRadius;
        SpawnAttackEffect(center, dir, attackRadius);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            attackRadius,
            enemyLayer
        );

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        bool didHit = false;

        foreach (var hit in hits)
        {
            IDamageable target = GetDamageable(hit);
            if (target == null)
                continue;

            if (hitTargets.Contains(target))
                continue;

            Vector2 toTarget =
                ((Vector2)hit.transform.position - origin).normalized;

            if (Vector2.Dot(dir, toTarget) < 0.3f)
                continue;

            target.TakeDamage(damage);
            target.ApplyKnockback(toTarget, 3f, 0.15f);

            hitTargets.Add(target);
            didHit = true;
        }

        if (didHit)
            CameraShake.Shake(0.11f, 0.1f);

        comboIndex = (comboIndex + 1) % 3;
    }

    private IDamageable GetDamageable(Collider2D hit)
    {
        if (hit.TryGetComponent(out IDamageable target))
            return target;

        return hit.GetComponentInParent<IDamageable>();
    }

    private void SpawnAttackEffect(Vector2 position, Vector2 dir, float attackRadius)
    {
        GameObject prefab = GetEffectPrefab();
        if (prefab == null)
            return;

        float angle = dir.sqrMagnitude > 0.001f
            ? Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + EffectAngleOffset
            : 0f;

        GameObject effect = Object.Instantiate(prefab, position, Quaternion.Euler(0f, 0f, angle));
        float scale = GetScaleForEffectRadius(effect, attackRadius);
        effect.transform.localScale = new Vector3(scale, scale, 1f);

        Object.Destroy(effect, GetEffectLifetime(effect));
    }

    private float GetScaleForEffectRadius(GameObject effect, float attackRadius)
    {
        SpriteRenderer renderer = effect.GetComponentInChildren<SpriteRenderer>();
        float attackDiameter = attackRadius * 2f * EffectRangeVisualScale;
        if (renderer == null || renderer.sprite == null)
            return Mathf.Max(EffectMinScale, attackDiameter);

        Vector2 spriteSize = renderer.sprite.bounds.size;
        Vector3 rendererScale = renderer.transform.lossyScale;
        float baseDiameter = Mathf.Max(
            spriteSize.x * Mathf.Abs(rendererScale.x),
            spriteSize.y * Mathf.Abs(rendererScale.y)
        );
        if (baseDiameter <= 0f)
            return Mathf.Max(EffectMinScale, attackDiameter);

        float currentScale = Mathf.Max(Mathf.Abs(effect.transform.localScale.x), Mathf.Abs(effect.transform.localScale.y));
        if (currentScale <= 0f)
            currentScale = 1f;

        return Mathf.Max(EffectMinScale, currentScale * attackDiameter / baseDiameter);
    }

    private GameObject GetEffectPrefab()
    {
        if (effectPrefabs == null || effectPrefabs.Length == 0)
            return null;

        int index = Mathf.Clamp(comboIndex, 0, effectPrefabs.Length - 1);
        if (effectPrefabs[index] != null)
            return effectPrefabs[index];

        for (int i = 0; i < effectPrefabs.Length; i++)
        {
            if (effectPrefabs[i] != null)
                return effectPrefabs[i];
        }

        return null;
    }

    private float GetEffectLifetime(GameObject effect)
    {
        const float fallbackLifetime = 0.45f;

        Animator animator = effect.GetComponent<Animator>();
        RuntimeAnimatorController controller = animator != null ? animator.runtimeAnimatorController : null;
        if (controller == null)
            return fallbackLifetime;

        float lifetime = 0f;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip != null)
                lifetime = Mathf.Max(lifetime, clip.length);
        }

        return lifetime > 0f ? lifetime : fallbackLifetime;
    }
}
