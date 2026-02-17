using System;
using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour, IDamageable
{
    private SpriteRenderer _renderer;
    private Enemy enemyStat;
    
    private float health;
    
    private Coroutine slowRoutine;
    private Coroutine knockRoutine;
    private float _x = 1;
    private int count;
    private float moveTimer = 0f;
    private Rigidbody2D rb;

    private bool isKnocked;
    private float knockMultiplier = 1f;
    
    private void Awake()
    {
        enemyStat = GetComponent<Enemy>();
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        health = enemyStat.stats.health;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
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
        yield return new WaitForSeconds(duration);
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

        rb.linearVelocity = new Vector2(
            xDir * power,
            2f
        );

        yield return new WaitForSeconds(duration);

        isKnocked = false;
        knockRoutine = null;
    }


    private IEnumerator Hit()
    {
        float time = 0;
        while (time < 0.3f)
        {
            _renderer.color = Color.red;
            time += Time.deltaTime;
            yield return null;
        }
        _renderer.color = Color.white;
    }
}
