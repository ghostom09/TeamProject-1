using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemyReset
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Sprite defaultSprite;
    private RuntimeAnimatorController defaultAnimatorController;
    private EnemyMove move;
    private EnemyAttack attack;
    private EnemyHit hit;
    
    public EnemyStats enemyStats;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            animator = CreateAnimatorOnVisual();
        if (spriteRenderer != null)
            defaultSprite = spriteRenderer.sprite;
        if (animator != null)
            defaultAnimatorController = animator.runtimeAnimatorController;

        move = GetComponent<EnemyMove>();
        attack = GetComponent<EnemyAttack>();
        hit = GetComponent<EnemyHit>();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        enemyStats = stats;

        ApplyVisuals(enemyStats);

        move.SetAnimator(animator);
        attack.SetAnimator(animator);
        hit.SetAnimator(animator);
        
        move.Init(enemyStats, target, m);
        attack.Init(enemyStats, target, m);
        hit.Init(enemyStats, target, m);
    }

    private void ApplyVisuals(EnemyStats stats)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = stats.sprite != null ? stats.sprite : defaultSprite;
            spriteRenderer.color = Color.white;
            spriteRenderer.enabled = spriteRenderer.sprite != null;
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator == null)
            animator = CreateAnimatorOnVisual();

        if (animator == null)
            return;

        animator.runtimeAnimatorController = stats.animatorController != null
            ? stats.animatorController
            : defaultAnimatorController;

        animator.enabled = animator.runtimeAnimatorController != null;
        if (animator.enabled)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    private Animator CreateAnimatorOnVisual()
    {
        GameObject animatorTarget = spriteRenderer != null
            ? spriteRenderer.gameObject
            : gameObject;

        Animator targetAnimator = animatorTarget.GetComponent<Animator>();
        if (targetAnimator == null)
            targetAnimator = animatorTarget.AddComponent<Animator>();

        return targetAnimator;
    }
}
