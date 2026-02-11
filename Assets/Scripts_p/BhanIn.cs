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

    private float stayTime;
    private bool isHovered;
    private BhanInRange detector;
    
    
    public void Init(GameObject user, float damage)
    {
        this.damage = damage;
        
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

        StartCoroutine(DamageOverTime(damage, 0.1f));
    }
    
    private IEnumerator DamageOverTime(float dmg, float interval)
    {
        if (detector == null)
        {
            Debug.LogError("BhanInRange(detector)가 없음! 구조 확인 필요");
            yield break;
        }
        while(true)
        {
            detector.DealDamageToAll(dmg);
            yield return new WaitForSeconds(interval);
        }
    }
}
