using UnityEngine;

public class GunNormalAttack : INormalAttack
{
    public float cooldown;
    private float lastTime;
    
    public void TryAttack(GameObject user, Vector2 dir)
    {
        if (Time.time < lastTime + cooldown)
            return;

        lastTime = Time.time;
        
        Shoot(user, dir);
    }

    private void Shoot(GameObject user, Vector2 dir)
    {
        Debug.Log("탕!");

        // 나중에 여기
        // - 총알 생성
        // - 히트스캔
        // - 반동
    }
}
