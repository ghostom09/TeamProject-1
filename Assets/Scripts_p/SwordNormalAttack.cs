using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordNormalAttack : INormalAttack
{
    private int comboIndex = 0;
    private float lastAttackTime;
    private float comboResetTime = 1.5f;
    
    
    private PlayerAttack attacker;
    private CharacterData data;
    

    public void Init(CharacterData data)
    {
        this.data = data;
    }
    private float GetComboRangeMultiplier()
    {
        return comboIndex switch
        {
            0 => 0.8f,
            1 => 1.0f,
            2 => 1.2f,
            _ => 1f
        };
    }
    private float GetComboDamageMultiplier()
    {
        return comboIndex switch
        {
            0 => 0.5f,
            1 => 0.7f,
            2 => 1.2f,
            _ => 1f
        };
    }
    public bool TryAttack(GameObject user, Vector2 dir, GameObject hitBox)
    {
        if (Time.time > lastAttackTime + comboResetTime)
            comboIndex = 0;

        float attackInterval = 1f / data.AttackSpeed;

        if (Time.time < lastAttackTime + attackInterval)
        {
            Debug.Log("공격 쿨타임!!!");
            return false;
        }

        lastAttackTime = Time.time;
        
        DoComboAttack(user, dir, hitBox);
        return true;
    }

    public void EndAttack(GameObject user, GameObject hitBox)
    {
        hitBox.SetActive(false);
    }

    private void DoComboAttack(GameObject user, Vector2 dir, GameObject hitBox)
    {
        hitBox.SetActive(true);
        
        float range = data.Range * 2 * GetComboRangeMultiplier();
        float damage = data.Damage * GetComboDamageMultiplier();

        HitBox(user, dir, range, hitBox);
        
        var hb = hitBox.GetComponent<HitBox>();
        hb.SetDamage(damage);
        
        comboIndex = (comboIndex + 1) % 3;
    }

    private void HitBox(GameObject user, Vector2 dir, float range, GameObject hitBox)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitBox.transform.rotation = Quaternion.Euler(0, 0, angle);
        hitBox.transform.localScale = new Vector3(range, range, 1);
        hitBox.transform.localPosition = dir.normalized * range/2;
    }
}
