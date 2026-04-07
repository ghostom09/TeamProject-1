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
    [SerializeField] private GameObject bigSwordIllusions;
    [SerializeField] private GameObject playerIllusions;
    [SerializeField] private SpriteRenderer me;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private RectTransform background;
    
    private List<GameObject> Illusions = new();
    private List<GameObject> fastBroken = new();
    private int illusionsCnt = 5;
    private Vector2 playerPos;
    
    public float radius = 6f;
    
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
        playerPos = pos;
        StartCoroutine(UltimateRoutine());
    }

    private IEnumerator UltimateRoutine()
    {
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
            Vector2 pos = playerPos + posOffsets[i];
            Vector2 dir = playerPos + dirOffsets[i];

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (obj)
            {
                Destroy(obj);
            }
            obj = Instantiate(swordIllusions, pos, Quaternion.Euler(0, 0, angle - 90f));
            
            if (obj.TryGetComponent(out SwordUltraAttack swordUltra))
            {
                swordUltra.Initialize(dir, 100);
            }

            Illusions.Add(Instantiate(playerIllusions, pos, Quaternion.identity));
            
            yield return new WaitForSeconds(0.12f);
        }
        Destroy(obj);
    }

    private IEnumerator Attacking()
    {
        GameObject obj = null;
        for (int i = 0; i < posOffsets.Length * 10; i++)
        {
            obj = SpawnSwordRandom();
            yield return new WaitForSeconds(0.015f);
        }
        Destroy(obj);
    }

    private IEnumerator EndOfAttack()
    {
        foreach (var sword in Illusions)
        {
            Destroy(sword);
        }
        foreach (var sword in fastBroken)
        {
            Destroy(sword);
        }
        GameObject bigSword = Instantiate(bigSwordIllusions, playerPos, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        Destroy(bigSword);

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
}
