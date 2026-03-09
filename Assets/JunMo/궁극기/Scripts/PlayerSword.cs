using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private CameraMove cam;
    [SerializeField] private GameObject swordIllusions;
    [SerializeField] private GameObject swordIllusions2;
    [SerializeField] private GameObject bigSwordIllusions;
    [SerializeField] private GameObject playerIllusions;
    [SerializeField] private SpriteRenderer me;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private RectTransform background;
    
    private List<SwordUltraAttack> swordUltraAttacks = new();
    private List<GameObject> ultraObject = new();
    
    public float radius = 5f;
    
    Vector2[] posOffsets = new Vector2[]
    {
        new Vector2(2, 3),
        new Vector2(-3.25f, 2.2f),
        new Vector2(0.9f, 3.8f),
    };

    Vector2[] dirOffsets = new Vector2[]
    {
        new Vector2(-3.8f, -0.95f),
        new Vector2(3.7f, -1),
        new Vector2(-2.5f, 2.85f),
    };

    
    public void Ultimate()
    {
        StopAllCoroutines();
        StartCoroutine(UltimateRoutine());
    }

    private IEnumerator UltimateRoutine()
    {
        StartCoroutine(Background());
        yield return new WaitForSeconds(0.1f);
        me.enabled = false;
        trail.enabled = false;
        StartCoroutine(StartAttack());
        yield return new WaitForSeconds(0.6f);
        me.enabled = true;
        trail.enabled = true;
        StartCoroutine(Attacking());
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(EndOfAttack());
    }

    private IEnumerator Background()
    {
        float time = 0f;
        float duration = 1f;

        background.anchorMin = new Vector2(0, 0);
        background.anchorMax = new Vector2(0, 1);

        while (time < duration)
        {
            time += Time.time;
            float progress = Mathf.Clamp01(time / duration);

            background.anchorMax = new Vector2(progress, 1);

            yield return null;
        }

        background.anchorMax = new Vector2(1, 1);
    }

    private IEnumerator StartAttack()
    {
        for (int i = 0; i < posOffsets.Length; i++)
        {
            Vector2 pos = (Vector2)transform.position + posOffsets[i];
            Vector3 dir = (Vector2)transform.position + dirOffsets[i] - pos;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject obj = Instantiate(swordIllusions, pos, Quaternion.Euler(0, 0, angle - 90f));
            SwordUltraAttack swordUltra = obj.GetComponent<SwordUltraAttack>();

            swordUltra.Initialize(dir, 50);
            ultraObject.Add(obj);

            ultraObject.Add(Instantiate(playerIllusions, pos, Quaternion.identity));
            
            Vector2 randomStart = (Vector2)transform.position + Random.insideUnitCircle * 5.5f;
            Vector2 randomTarget = (Vector2)transform.position + Random.insideUnitCircle * 5.5f;

            GameObject obj2 = Instantiate(swordIllusions2, randomStart, Quaternion.identity);
            SwordUltraAttack sword = obj2.GetComponent<SwordUltraAttack>();
            ultraObject.Add(obj2);
            sword.Initialize(randomTarget, 50);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Attacking()
    {
        for (int i = 0; i < posOffsets.Length; i++)
        {
            float angle1 = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float angle2 = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector2 point1 = (Vector2)transform.position + new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;
            Vector2 point2 = (Vector2)transform.position + new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;
            
            Vector2 pos = (Vector2)transform.position + point1;
            Vector3 dir = (Vector2)transform.position + point2;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject obj = Instantiate(swordIllusions, pos, Quaternion.Euler(0, 0, angle - 90f));
            SwordUltraAttack swordUltra = obj.GetComponent<SwordUltraAttack>();

            swordUltra.Initialize(dir, 50);
            ultraObject.Add(obj);

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.4f);
        foreach (var sword in ultraObject)
        {
            Destroy(sword);
        }
    }

    private IEnumerator EndOfAttack()
    {
        GameObject swords = Instantiate(bigSwordIllusions, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        Destroy(swords);

        yield return null;
    }
    
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Ultimate();
        }
    }
}
