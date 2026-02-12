using System.Collections.Generic;
using UnityEngine;

public class BhanInRange : MonoBehaviour
{
    private List<IDamageable> enemies = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable dmg))
        {
            enemies.Add(dmg);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable dmg))
        {
            enemies.Remove(dmg);
        }
    }

    public void DealDamageToAll(float damage, float slowPercent)
    {
        if (enemies == null || enemies.Count == 0)
        {
            Debug.Log("범위 내 적 없음");
            return;
        }
        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(damage);
            enemy.ApplySlow(slowPercent, 0.2f);
        }
        Debug.Log("공격중!!!");
    }
    


}
