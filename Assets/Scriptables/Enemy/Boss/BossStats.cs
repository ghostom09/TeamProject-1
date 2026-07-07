using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BossType
{
    None,
    warrior,
    magician
}

[CreateAssetMenu(fileName = "BossStats")]

public class BossStats : ScriptableObject
{
    public BossType bossType;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
    public bool useSpriteBoundsForCollider = true;
    public Vector2 colliderOffset;
    public Vector2 colliderSize;
    public float health;
    public float speed;
    public float jumpForce;
    public int exp;
    
    public float skillInterval;
    
    public List<BossSkills> skills;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sprite != null && bossType != BossType.magician)
            return;

        string spritePath = bossType switch
        {
            BossType.warrior => "Assets/Sprites/Enemy/SwordBoss.aseprite",
            BossType.magician => "Assets/Sprites/Enemy/magician_boss.aseprite",
            _ => null
        };

        if (string.IsNullOrEmpty(spritePath))
            return;

        Sprite foundSprite = FindSpriteAtPath(spritePath);
        if (foundSprite == null || sprite == foundSprite)
            return;

        sprite = foundSprite;
        EditorUtility.SetDirty(this);
    }

    private static Sprite FindSpriteAtPath(string spritePath)
    {
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
        return renderer != null ? renderer.sprite : null;
    }
#endif
}
