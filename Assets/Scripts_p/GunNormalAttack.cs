using UnityEngine;

public class GunNormalAttack : INormalAttack
{
    
    private float lastTime;
    private CharacterData data;

    public void Init(CharacterData data)
    {
        this.data = data;
    }
    
    public bool TryAttack(GameObject user, Vector2 dir, GameObject hitBox)
    {
        if (Time.time < lastTime + 1 / data.AttackSpeed)
        {
            Debug.Log("공격 쿨타임!!!!!!!");
            return false;
        }
            

        lastTime = Time.time;
        
        Shoot(user, dir, hitBox);
        return true;
    }

    public void EndAttack(GameObject user, GameObject hitBox)
    {
        
    }

    private void Shoot(GameObject user, Vector2 dir, GameObject hitBox)
    {
        Debug.Log($"공격력 : {data.Damage}, 사거리 : {data.Range}");
        
        Debug.DrawRay(user.transform.position, dir.normalized * data.Range, Color.cyan);
        // 나중에 여기
        // - 총알 생성
        // - 히트스캔
        // - 반동
    }
}
