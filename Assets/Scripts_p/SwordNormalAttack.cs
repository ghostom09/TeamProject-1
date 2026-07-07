using System.Collections.Generic;
using UnityEngine;

public class SwordNormalAttack : INormalAttack
{
    private int comboIndex = 0;
    private float lastAttackTime;
    private const float ComboResetTime = 1.5f;
    private const float EffectAngleOffset = -170f;

    private CharacterData data;
    private GameObject[] effectPrefabs;

    private LayerMask enemyLayer;

    public void Init(CharacterData data)
    {
        this.data = data;
        enemyLayer = LayerMask.GetMask("Enemy");
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

        DoComboAttack(player, user, dir);

        return true;
    }

    private void DoComboAttack(Player player, GameObject user, Vector2 dir)
    {
        Vector2 origin = user.transform.position;

        float range = data.Range * GetComboRangeMultiplier();
        float damage = player.Stats.Damage * GetComboDamageMultiplier();

        Vector2 center = origin + dir * range * 0.5f;
        SpawnAttackEffect(center, dir, range);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            range * 0.5f,
            enemyLayer
        );

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        bool didHit = false;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable target))
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

    private void SpawnAttackEffect(Vector2 position, Vector2 dir, float range)
    {
        GameObject prefab = GetEffectPrefab();
        if (prefab == null)
            return;

        float angle = dir.sqrMagnitude > 0.001f
            ? Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + EffectAngleOffset
            : 0f;

        GameObject effect = Object.Instantiate(prefab, position, Quaternion.Euler(0f, 0f, angle));
        float scale = GetScaleForEffectDiameter(effect, range);
        effect.transform.localScale = new Vector3(scale, scale, 1f);

        Object.Destroy(effect, GetEffectLifetime(effect));
    }

    private float GetScaleForEffectDiameter(GameObject effect, float diameter)
    {
        const float minScale = 0.1f;

        SpriteRenderer renderer = effect.GetComponentInChildren<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null)
            return Mathf.Max(minScale, diameter);

        Vector2 spriteSize = renderer.sprite.bounds.size;
        float baseDiameter = Mathf.Max(spriteSize.x, spriteSize.y);
        if (baseDiameter <= 0f)
            return Mathf.Max(minScale, diameter);

        return Mathf.Max(minScale, diameter / baseDiameter);
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
