using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private CameraMove cam;
    [SerializeField] private GameObject swordIllusions;
    [SerializeField] private GameObject swordIllusions2;
    [SerializeField] private GameObject bigSwordIllusions;
    [SerializeField] private SpriteRenderer me;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private RectTransform background;
    [SerializeField] private int backgroundSortingOrder = -1000;
    [SerializeField] private Vector2 backgroundMapCenter = Vector2.zero;
    [SerializeField] private Vector2 backgroundMapSize = new Vector2(120f, 80f);
    
    private Vector2 playerPos;
    private SpriteRenderer worldBackground;
    private float backgroundProgress;
    private bool isUltimateRunning;
    
    public float radius = 6f;

    private void Awake()
    {
        CreateWorldBackground();
        HideUiBackground();
    }
    
    Vector2[] posOffsets = new Vector2[]
    {
        new Vector2(1.83f, -3.28f),
        new Vector2(-3.48f, 1.90f),
        new Vector2(3.82f, 1.06f),
        new Vector2(-3.04f, -2.36f),
        new Vector2(0.22f, 3.89f)
    };

    Vector2[] dirOffsets = new Vector2[]
    {
        new Vector2(-3.48f, 1.90f),
        new Vector2(3.82f, 1.06f),
        new Vector2(-3.04f, -2.36f),
        new Vector2(0.22f, 3.89f),
        new Vector2(1.83f, -3.28f),
    };
    
    public void Ultimate(Vector2 pos)
    {
        if (isUltimateRunning)
            return;

        playerPos = pos;
        StartCoroutine(UltimateRoutine());
    }

    private IEnumerator UltimateRoutine()
    {
        isUltimateRunning = true;
        StartCoroutine(Background());
        yield return new WaitForSeconds(0.1f);
        
        me.enabled = false;
        trail.enabled = false;
        
        StartCoroutine(StartAttack());
        yield return new WaitForSeconds(0.5f);
        
        me.enabled = true;
        trail.enabled = true;
        
        StartCoroutine(Attacking());
        yield return new WaitForSeconds(1.4f);
        yield return new WaitForSeconds(0.8f);
        yield return EndOfAttack();
        Reset();
        
    }

    private IEnumerator Background()
    {
        float time = 0f;
        float duration = 1f;

        PrepareWorldBackground(0f);

        while (time < duration)
        {
            time += Time.deltaTime * 7;
            float progress = Mathf.Clamp01(time / duration);

            SetWorldBackgroundProgress(progress);

            yield return null;
        }

        SetWorldBackgroundProgress(1f);
    }

    private IEnumerator StartAttack()
    {
        GameObject obj = null;
        for (int i = 0; i < posOffsets.Length; i++)
        {
            Vector2 pos = playerPos + posOffsets[i];
            Vector2 dir = playerPos + dirOffsets[i];

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (obj)
            {
                ObjectPoolManager.Instance.Release(ObjectName.UltraSwordIllusions, obj);
            }
            obj = ObjectPoolManager.Instance.Get
                (ObjectName.UltraSwordIllusions, pos, Quaternion.Euler(0, 0, angle - 90f));
            
            if (obj.TryGetComponent(out SwordUltraAttack swordUltra))
            {
                swordUltra.Initialize(dir, 100);
            }

            yield return new WaitForSeconds(0.12f);
        }
        ObjectPoolManager.Instance.Release(ObjectName.UltraSwordIllusions, obj);
    }

    private IEnumerator Attacking()
    {
        GameObject obj = null;
        for (int i = 0; i < posOffsets.Length * 10; i++)
        {
            obj = SpawnSwordRandom();
            yield return new WaitForSeconds(0.015f);
        }
        ObjectPoolManager.Instance.Release(ObjectName.UltraSwordBigIllusions, obj);
    }

    private IEnumerator EndOfAttack()
    {
        GameObject bigSword = ObjectPoolManager.Instance.
            Get(ObjectName.UltraSwordFinal, playerPos, Quaternion.identity);

        float finalDuration = GetAnimationDuration(bigSword, 0.5f);
        RestartAnimator(bigSword);

        yield return new WaitForSeconds(finalDuration);
        ObjectPoolManager.Instance.Release(ObjectName.UltraSwordFinal, bigSword);

        yield return null;
    }

    private GameObject SpawnSwordRandom()
    {
        float angle1 = Random.Range(0f, 360f);
        float offset = Random.Range(140f, 220f);

        float rad = Mathf.Deg2Rad;

        float angle3 = angle1 * rad;
        float angle2 = (angle1 + offset) * rad;

        Vector2 pos = playerPos + 
                      new Vector2(Mathf.Cos(angle3), Mathf.Sin(angle3)) * radius + 
                      new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector2 dir = playerPos + 
                      new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius + 
                      new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = ObjectPoolManager.Instance.Get
            (ObjectName.UltraSwordBigIllusions, pos, Quaternion.Euler(0, 0, angle - 90f));
        
        if (obj.TryGetComponent(out SwordUltraAttack swordUltra))
        {
            swordUltra.Initialize(dir, 100);
        }

        // StartCoroutine(Die(obj));
        
        return obj;
    }

    // private IEnumerator Die(GameObject obj)
    // {
    //     yield return new WaitForSeconds(0.5f);
    //     ObjectPoolManager.Instance.Release(ObjectName.UltraSwordBigIllusions, obj);
    // }

    private void Reset()
    {
        StopAllCoroutines();
        SetWorldBackgroundProgress(0f);
        isUltimateRunning = false;
    }

    private float GetAnimationDuration(GameObject obj, float fallback)
    {
        Animator animator = obj != null ? obj.GetComponentInChildren<Animator>() : null;
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips != null && clips.Length > 0)
                return clips[0].length;
        }

        return fallback;
    }

    private void RestartAnimator(GameObject obj)
    {
        Animator animator = obj != null ? obj.GetComponentInChildren<Animator>() : null;
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    private void HideUiBackground()
    {
        if (background != null && background.TryGetComponent(out Image image))
        {
            image.enabled = false;
        }
    }

    private void CreateWorldBackground()
    {
        if (worldBackground != null)
            return;

        GameObject obj = new GameObject("SwordUltimateWorldBackground");
        obj.transform.SetParent(transform, false);

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );

        worldBackground = obj.AddComponent<SpriteRenderer>();
        worldBackground.sprite = sprite;
        worldBackground.color = Color.black;
        worldBackground.sortingOrder = backgroundSortingOrder;
        worldBackground.enabled = false;
    }

    private void PrepareWorldBackground(float progress)
    {
        if (worldBackground == null)
            CreateWorldBackground();

        worldBackground.enabled = true;
        SetWorldBackgroundProgress(progress);
    }

    private void SetWorldBackgroundProgress(float progress)
    {
        if (worldBackground == null)
            return;

        backgroundProgress = Mathf.Clamp01(progress);
        UpdateWorldBackgroundTransform();
    }

    private void UpdateWorldBackgroundTransform()
    {
        if (worldBackground == null)
            return;

        float width = backgroundMapSize.x;
        float height = backgroundMapSize.y;

        worldBackground.transform.position = new Vector3(
            backgroundMapCenter.x - width * 0.5f + width * backgroundProgress * 0.5f,
            backgroundMapCenter.y,
            0f
        );
        worldBackground.transform.localScale = new Vector3(width * backgroundProgress, height, 1f);
        worldBackground.sortingOrder = backgroundSortingOrder;
        worldBackground.enabled = backgroundProgress > 0f;
    }
}
