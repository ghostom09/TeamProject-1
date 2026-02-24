using System;
using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour, IDamageable, IEnemyReset
{
    private SpriteRenderer renderer;
    private EnemyMove enemyMove;
    private EnemySpawnerManager spawnerManager;
    
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
        renderer = GetComponent<SpriteRenderer>();
        enemyMove = GetComponent<EnemyMove>();
    }

    public void Init(EnemyStats stats, GameObject target, EnemySpawnerManager m)
    {
        health = stats.health;
        spawnerManager = m;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        StartCoroutine(Hit());
        
        if (health <= 0)
        {
            Die();
        }
    }

    public void ApplySlow(float slowPercent, float slowDuration)
    {
        if(!gameObject.activeInHierarchy)
            return;
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
        while (time < 0.5f)
        {
            renderer.color = Color.red;
            time += Time.deltaTime;
            yield return null;
        }
        renderer.color = Color.white;
    }
    
    private void Die()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);

        spawnerManager.ReturnToPool(GetComponent<Enemy>());
    }
}
