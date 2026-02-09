using UnityEngine;

public class SwordNormalAttack : INormalAttack
{
    private int comboIndex = 0;
    private float lastAttackTime;

    private float comboResetTime = 0.7f;

    // 콤보별 쿨타임
    private float[] comboCooldowns = { 0.3f, 0.35f, 0.5f};

    public void TryAttack(GameObject user, Vector2 dir)
    {
        if (Time.time > lastAttackTime + comboResetTime)
        {
            comboIndex = 0;
        }
        
        if (Time.time < lastAttackTime + comboCooldowns[comboIndex])
            return;

        lastAttackTime = Time.time;

        DoComboAttack(user, dir);
    }

    private void DoComboAttack(GameObject user, Vector2 dir)
    {
        switch (comboIndex)
        {
            case 0:
                Debug.Log("검 1타!");
                break;

            case 1:
                Debug.Log("검 2타!");
                break;

            case 2:
                Debug.Log("검 3타!");
                break;
        }

        comboIndex = (comboIndex + 1) % comboCooldowns.Length;
    }
}
