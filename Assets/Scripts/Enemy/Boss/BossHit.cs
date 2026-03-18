using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHit : MonoBehaviour, IBossReset, IDamageable
{
    [SerializeField] private float health;
    private float maxHealth;
    private SpriteRenderer renderer;
    private Color color;
    private int shield;
    [SerializeField] private float reductionRate;
    [SerializeField] private GameObject Shield;
    
    private BossMove _bossMove;

    private int exp;

    private PlayerLevelManager levelManager;
    
    [SerializeField] private RectTransform healthBar;
    private float maxHealthBar;

    private void Awake()
    {
        _bossMove = GetComponent<BossMove>();
        renderer = GetComponent<SpriteRenderer>();
        maxHealthBar = healthBar.sizeDelta.x;
    }

    public void Init(BossStats stats, GameObject target, EnemySpawnerManager m)
    {
        health = stats.health;
        maxHealth = stats.health;
        exp = stats.exp;
        color = renderer.color;
        healthBar.sizeDelta = new Vector2(maxHealthBar, healthBar.sizeDelta.y);

        levelManager = target.GetComponent<PlayerLevelManager>();
    }

    public void TakeDamage(float dmg)
    {
        if (shield > 0)
        {
            shield--;
            dmg *= 1-reductionRate;
            if(shield <= 0)
                Shield.SetActive(false);
        }
        health -= dmg;
        StartCoroutine(Hit());
        
        healthBar.sizeDelta = new Vector2(health / maxHealth * maxHealthBar, healthBar.sizeDelta.y);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        levelManager.AddExp(exp);

        gameObject.SetActive(false);
    }
    
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
    
    public void MakeShield(int shieldStock)
    {
        Shield.SetActive(true);
        shield = shieldStock;
    }
    
    public void ApplySlow(float slowPercent, float slowDuration) { }
    
    public void ApplyKnockback(Vector2 dir, float power, float duration) {_bossMove.ApplyKnockback(dir, power, duration);}
}
