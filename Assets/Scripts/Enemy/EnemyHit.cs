using System;
using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour, IDamageable
{
    private SpriteRenderer renderer;
    private Enemy enemyStat;
    private EnemyMove enemyMove;
    
    private float health;
    
    private Coroutine slowRoutine;
    private Coroutine knockRoutine;
    private float _x = 1;
    private int count;
    private float moveTimer = 0f;

    private bool isKnocked;
    private float knockMultiplier = 1f;
    
    private void Awake()
    {
        enemyStat = GetComponent<Enemy>();
        renderer = GetComponent<SpriteRenderer>();
        enemyMove = GetComponent<EnemyMove>();
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

    public void ApplyKnockback(Vector2 dir, float power, float duration){enemyMove.ApplyKnockback(dir, power, duration);}


    private IEnumerator Hit()
    {
        float time = 0;
        while (time < 0.3f)
        {
            renderer.color = Color.red;
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = Color.white;
    }
}
