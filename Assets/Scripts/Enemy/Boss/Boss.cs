using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Boss : MonoBehaviour, IBossReset
{
    private BossMove move;
    private BossAttack attack;
    private BossHit hit;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private Sprite defaultSprite;
    private RuntimeAnimatorController defaultAnimatorController;
    private Vector2 defaultColliderOffset;
    private Vector2 defaultColliderSize;
    private GameObject targetObj;
    public EnemySpawnerManager manager;

    private BossStats bossStats;
    
    private void Awake()
    {
        move = GetComponent<BossMove>();
        attack = GetComponent<BossAttack>();
        hit = GetComponent<BossHit>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer != null)
            defaultSprite = spriteRenderer.sprite;

        if (animator != null)
            defaultAnimatorController = animator.runtimeAnimatorController;

        if (boxCollider != null)
        {
            defaultColliderOffset = boxCollider.offset;
            defaultColliderSize = boxCollider.size;
        }
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        bossStats = stats;
        manager = m;
        targetObj = target;

        ApplyAnimatorController(bossStats);
        ApplySprite(bossStats);
        ApplyAliveAnimatorState(bossStats);
        ApplyCollider(bossStats);
        StartCoroutine(ApplyVisualNextFrame(bossStats));
        
        move.Init(bossStats, targetObj, manager);
        attack.Init(bossStats, targetObj, manager);
        hit.Init(bossStats, targetObj, manager);
    }

    private void ApplyAnimatorController(BossStats stats)
    {
        if (animator != null)
        {
            animator.runtimeAnimatorController = stats.animatorController != null 
                ? stats.animatorController 
                : defaultAnimatorController;

            animator.Rebind();
            animator.Update(0f);
        }
    }

    private void ApplySprite(BossStats stats)
    {
        if (spriteRenderer == null)
            return;

        Sprite fallbackSprite = LoadEditorFallbackSprite(stats.bossType);
        Sprite bossSprite = stats.sprite != null ? stats.sprite : fallbackSprite;

        if (stats.bossType == BossType.magician && fallbackSprite != null)
            bossSprite = fallbackSprite;

        spriteRenderer.sprite = bossSprite != null ? bossSprite : defaultSprite;
        spriteRenderer.enabled = spriteRenderer.sprite != null;
    }

    private void ApplyAliveAnimatorState(BossStats stats)
    {
        if (animator == null)
            return;

        animator.enabled = stats.bossType != BossType.magician;
    }

    private Sprite LoadEditorFallbackSprite(BossType bossType)
    {
#if UNITY_EDITOR
        string spritePath = bossType switch
        {
            BossType.warrior => "Assets/Sprites/Enemy/SwordBoss.aseprite",
            BossType.magician => "Assets/Sprites/Enemy/magician_boss.aseprite",
            _ => null
        };

        if (string.IsNullOrEmpty(spritePath))
            return null;

        Sprite spriteAsset = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (spriteAsset != null)
            return spriteAsset;

        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(spritePath))
        {
            if (asset is Sprite sprite)
                return sprite;
        }

        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(spritePath);
        if (prefabAsset == null)
            return null;

        SpriteRenderer renderer = prefabAsset.GetComponentInChildren<SpriteRenderer>(true);
        if (renderer != null)
            return renderer.sprite;
#endif

        return null;
    }

    private IEnumerator ApplyVisualNextFrame(BossStats stats)
    {
        yield return null;

        ApplySprite(stats);
        ApplyCollider(stats);
    }

    private void ApplyCollider(BossStats stats)
    {
        if (boxCollider == null)
            return;

        if (stats.colliderSize != Vector2.zero)
        {
            boxCollider.offset = stats.colliderOffset;
            boxCollider.size = stats.colliderSize;
            return;
        }

        if (stats.useSpriteBoundsForCollider && spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Bounds bounds = spriteRenderer.sprite.bounds;
            boxCollider.offset = bounds.center;
            boxCollider.size = bounds.size;
            return;
        }

        boxCollider.offset = defaultColliderOffset;
        boxCollider.size = defaultColliderSize;
    }
}
