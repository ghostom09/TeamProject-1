using System.Collections;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour, IDamageable
{
    private SpriteRenderer _renderer;
    public float hp;
    
    public float distance = 3f;

    [SerializeField] private float moveSpeed = 8;
    private Vector3 startPos;

    private float currentSpeed;
    private Coroutine slowRoutine;
    private Coroutine knockRoutine;
    private float _x = 1;
    private int count;
    private float moveTimer = 0f;
    private Rigidbody2D rb;
    private bool isKnocked;
    private float knockMultiplier = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isKnocked) return;   // 넉백 중엔 AI 이동 중단

        float finalSpeed = currentSpeed;

        float x = Mathf.PingPong(Time.time * finalSpeed, distance * 2) - distance;
        float targetX = startPos.x + x;

        float moveDir = targetX - transform.position.x;

        rb.linearVelocity = new Vector2(moveDir * 5f, rb.linearVelocity.y); */
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        StartCoroutine(Hit());
    }

    public void ApplySlow(float slowPercent, float slowDuration)
    {
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(Slow(slowPercent, slowDuration));
    }

    private IEnumerator Slow(float percent, float duration)
    {
        currentSpeed = moveSpeed * (1 - percent / 100f);

        yield return new WaitForSeconds(duration);

        currentSpeed = moveSpeed;
        slowRoutine = null;
    }
    public void ApplyKnockback(Vector2 dir, float power, float duration)
    {
        if (knockRoutine != null)
            StopCoroutine(knockRoutine);

        knockRoutine = StartCoroutine(Knockback(dir, power, duration));
    }

    private IEnumerator Knockback(Vector2 dir, float power, float duration)
    {
        isKnocked = true;
        
        float xDir = Mathf.Sign(dir.x);

        // 살짝 위로 뜨는 맛
        rb.linearVelocity = new Vector2(
            xDir * power,
            2f   // 약한 위쪽 힘
        );

        yield return new WaitForSeconds(duration);

        isKnocked = false;
        knockRoutine = null;
    }


    private IEnumerator Hit()
    {
        float time = 0;
        while (time < 0.05f)
        {
            _renderer.color = Color.red;
            time += Time.deltaTime;
            yield return null;
        }
        _renderer.color = Color.white;
    }
    
    
}
