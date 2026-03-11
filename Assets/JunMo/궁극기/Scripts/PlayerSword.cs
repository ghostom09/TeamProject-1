using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private CameraMove cam;
    [SerializeField] private GameObject swordIllusions;
    [SerializeField] private GameObject swordIllusions2;
    [SerializeField] private GameObject attackIllusions;
    [SerializeField] private GameObject attackAfterImage;
    [SerializeField] private GameObject bigSwordIllusions;
    [SerializeField] private GameObject playerIllusions;
    [SerializeField] private SpriteRenderer me;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private RectTransform background;
    
    private List<GameObject> Illusions = new();
    private List<GameObject> swords = new();
    private List<GameObject> fastBroken = new();
    private int illusionsCnt = 5;
    
    public float radius = 4f;
    
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
    // private List<SwordIllusionsAttack> _illusions 
    //     = new List<SwordIllusionsAttack>();
    // private Transform _pivot; //asd
    // public void SpawnIllusions(float damage)
    // {
    //     ClearIllusions();
    //
    //     int count = 3;
    //     float radius = 0.7f;
    //     float totalAngle = 80f;
    //
    //     _pivot = new GameObject("IllusionPivot").transform;
    //     _pivot.position = transform.position;
    //     for (int i = 0; i < count; i++)
    //     {
    //         float angle = i * (360f / count);
    //         float radian = angle * Mathf.Deg2Rad;
    //
    //         Vector3 offset = new Vector3(
    //             Mathf.Cos(radian),
    //             Mathf.Sin(radian),
    //             0f
    //         ) * radius;
    //
    //         Vector3 spawnPos = transform.position + offset;
    //
    //         GameObject obj = Instantiate(
    //             illusionPrefab,
    //             spawnPos,
    //             Quaternion.Euler(0, 0, angle - 90f)
    //         );
    //
    //         var illusion = obj.GetComponent<SwordIllusionsAttack>();
    //         illusion.Init(damage);
    //         obj.transform.SetParent(_pivot);
    //
    //         _illusions.Add(illusion);
    //     }
    // }
    //
    // void Update()
    // {
    //     if (_pivot != null)
    //     {
    //         _pivot.Rotate(0, 0, 30f * Time.deltaTime);
    //     }
    // }

    
    public void Ultimate()
    {
        StartCoroutine(UltimateRoutine());
    }

    private IEnumerator UltimateRoutine()
    {
        StartCoroutine(Background());
        yield return new WaitForSeconds(0.15f);
        me.enabled = false;
        trail.enabled = false;
        StartCoroutine(StartAttack());
        yield return new WaitForSeconds(0.8f);
        me.enabled = true;
        trail.enabled = true;
        StartCoroutine(Attacking());
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(EndOfAttack());
        yield return new WaitForSeconds(0.56f);
        Reset();
        
    }

    private IEnumerator Background()
    {
        float time = 0f;
        float duration = 1f;

        background.anchorMin = new Vector2(0, 0);
        background.anchorMax = new Vector2(0, 1);

        while (time < duration)
        {
            time += Time.deltaTime * 7;
            float progress = Mathf.Clamp01(time / duration);

            background.anchorMax = new Vector2(progress, 1);

            yield return null;
        }

        background.anchorMax = new Vector2(1, 1);
    }

    private IEnumerator StartAttack()
    {
        GameObject obj = null;
        for (int i = 0; i < posOffsets.Length; i++)
        {
            Vector2 pos = (Vector2)transform.position + posOffsets[i];
            Vector2 dir = (Vector2)transform.position + dirOffsets[i];

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (obj != null)
            {
                Destroy(obj);
            }
            obj = Instantiate(swordIllusions, pos, Quaternion.Euler(0, 0, angle - 90f));

            if (obj.TryGetComponent(out SwordUltraAttack swordUltra))
            {
                swordUltra.Initialize(dir, 100);
            }
            
            GameObject obj2 = Instantiate(attackAfterImage, pos, Quaternion.Euler(0, 0, angle - 90f));
            if (obj2.TryGetComponent(out SwordUltraAttack sword))
            {
                sword.Initialize(dir, 100);
            }
            swords.Add(obj2);

            Illusions.Add(Instantiate(playerIllusions, pos, Quaternion.identity));
            
            SpawnAttackRandom();
            yield return new WaitForSeconds(0.17f);
        }
        Destroy(obj);
    }

    private IEnumerator Attacking()
    {
        SpawnAttackRandom();
        GameObject obj = null;
        for (int i = 0; i < posOffsets.Length * 2; i++)
        {
            obj = SpawnSwordRandom();
            yield return new WaitForSeconds(0.07f);
        }
        Destroy(obj);
    }

    private IEnumerator EndOfAttack()
    {
        foreach (var sword in Illusions)
        {
            Destroy(sword);
        }
        foreach (var sword in swords)
        {
            Destroy(sword);
        }
        GameObject bigSword = Instantiate(bigSwordIllusions, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        Destroy(bigSword);

        yield return null;
    }

    private IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private void SpawnAttackRandom()
    {
        Vector2 randomStart = (Vector2)transform.position + Random.insideUnitCircle * 5.5f;
        Vector2 randomTarget = (Vector2)transform.position + Random.insideUnitCircle * 5.5f;

        GameObject obj = Instantiate(attackIllusions, randomStart, Quaternion.identity);
        if (obj.TryGetComponent(out SwordUltraAttack sword))
        {
            sword.Initialize(randomTarget, 100);
        }
        fastBroken.Add(obj);
    }

    private GameObject SpawnSwordRandom()
    {
        float angle1 = Random.Range(0f, 360f);
        float offset = Random.Range(140f, 220f);

        float rad = Mathf.Deg2Rad;

        float angle3 = angle1 * rad;
        float angle2 = (angle1 + offset) * rad;

        Vector2 pos = (Vector2)transform.position + 
                      new Vector2(Mathf.Cos(angle3), Mathf.Sin(angle3)) * radius + 
                      new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        Vector2 dir = (Vector2)transform.position + 
                      new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius + 
                      new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(swordIllusions2, pos, Quaternion.Euler(0, 0, angle - 90f));
        
        if (obj.TryGetComponent(out SwordUltraAttack swordUltra))
        {
            swordUltra.Initialize(dir, 100);
        }
        
        Destroy(obj, 0.5f);
        return obj;
    }

    private void Reset()
    {
        StopAllCoroutines();
        background.anchorMin = new Vector2(0, 0);
        background.anchorMax = new Vector2(0, 1);
    }
    
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Ultimate();
        }
    }
}
