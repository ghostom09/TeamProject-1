using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordNormalAttack : INormalAttack
{
    private int comboIndex = 0;
    private float lastAttackTime;

    private float comboResetTime = 1.5f;

    private CharacterData data;

    public void Init(CharacterData data)
    {
        this.data = data;
    }
    public bool TryAttack(GameObject user, Vector2 dir, GameObject hitBox)
    {
        if (Time.time > lastAttackTime + comboResetTime)
        {
            comboIndex = 0;
        }

        if (Time.time < lastAttackTime + 1 / data.AttackSpeed)
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
        switch (comboIndex)
        {
            
            case 0:
                HitBox(user,dir, data.Range * 2 * 0.8f, hitBox);
                break;

            case 1:
                HitBox(user,dir, data.Range * 2, hitBox);
                break;

            case 2:
                HitBox(user,dir, data.Range * 2 * 1.2f, hitBox);
                break;
        }

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
