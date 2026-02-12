using System.Collections;
using UnityEngine;

public class BhanIn : MonoBehaviour
{
    [SerializeField] private float hoverStartTime = 1.5f;
    [SerializeField] private GameObject range;
    
    public float damage;
    public float lifeTime = 2f;

    private Vector2 ownerPos;
    private Rigidbody2D rb;
    private BhanInRange detector;

    private float slowPercent;
    private float stayTime;
    private bool isHovered;
    
    
    
    public void Init(GameObject user, float damage, float slowPercent)
    {
        this.damage = damage;
        this.slowPercent = slowPercent;
        
        rb = GetComponent<Rigidbody2D>();
        detector = GetComponentInChildren<BhanInRange>(true);
        range.SetActive(false);
        
        Destroy(gameObject, lifeTime);
    }
    
    void FixedUpdate()
    {
        if (isHovered) return;

        bool isStopped = rb.linearVelocity.sqrMagnitude < 0.09f;

        if (!isStopped)
        {
            stayTime = 0f;
            return;
        }

        stayTime += Time.fixedDeltaTime;

        if (stayTime >= hoverStartTime)
        {
            Hover();
            isHovered = true;
        }
    }
    
    private void Hover()
    {
        isHovered = true;

        range.SetActive(true);
        range.transform.localScale = Vector2.one * 10f;

        StartCoroutine(DamageOverTime(damage, 0.1f,slowPercent));
    }

    private IEnumerator DamageOverTime(float dmg, float interval,float slowPercent)
    {
        if (detector == null)
        {
            Debug.LogError("BhanInRange(detector)가 없음! 구조 확인 필요");
            yield break;
        }

        while (true)
        {
            detector.DealDamageToAll(dmg,slowPercent);
            yield return new WaitForSeconds(interval);
        }
    }
}
