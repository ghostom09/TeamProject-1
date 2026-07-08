using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DpsMeter : MonoBehaviour,IDamageable
{
    private float totalDamage;
    private Queue<(float damage, float time)> damageQueue = new();
    private SpriteRenderer _renderer;
    
    [SerializeField] private TextMeshProUGUI damageText;
    
    private float dps;
    private float allDamage;
    private float moveSpeed = 100;
    private float currentSpeed = 100;
    public Coroutine slowRoutine;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    private void RegisterDamage(float damage)
    {
        float now = Time.time;

        damageQueue.Enqueue((damage, now));
        totalDamage += damage;

        CleanupOldDamage(now);
    }

    private void Update()
    {
        CleanupOldDamage(Time.time);
        dps = totalDamage;

        damageText.text = "DPS : " + dps.ToString("0.00") + "\n"
                          + "TotalDamage : " + allDamage.ToString("0.00") + "\n"
                          + "MoveSpeed : " + currentSpeed.ToString("0.00") + "%" + "\n";
    }

    private void CleanupOldDamage(float currentTime)
    {
        while (damageQueue.Count > 0 && currentTime - damageQueue.Peek().time > 1f)
        {
            var old = damageQueue.Dequeue();
            totalDamage -= old.damage;
        }
    }

    public void TakeDamage(float damage)
    {
        RegisterDamage(damage);
        allDamage += damage;
        StartCoroutine(Hit());
    }

    public void ApplySlow(float percent, float duration)
    {
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(Slow(percent, duration));
    }

    private IEnumerator Slow(float percent, float duration)
    {
        currentSpeed = moveSpeed - percent;
        yield return new WaitForSeconds(duration);
        currentSpeed = moveSpeed;
    }

    public void ApplyKnockback(Vector2 dir, float power, float duration)
    {
        
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
