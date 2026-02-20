using Mono.Cecil.Cil;
using UnityEngine;

public class Sword2 : SkillBase
{
    private GameObject bladePrefab;
    private float throwSpeed = 8f;
    private float slowPercent = 40;

    public Sword2() : base(13f)
    {
        bladePrefab = Resources.Load<GameObject>("BhanIn");
    }

    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (player.isUsingUltimate)
            return;
        
        dir = dir.normalized;

        // 1. 생성
        GameObject blade = Object.Instantiate(
            bladePrefab,
            user.transform.position,
            Quaternion.identity
        );

        // 2. 초기화
        blade.GetComponent<BhanIn>()
            .Init(user , data.Damage * 0.1f, slowPercent);

        // 3. 발사
        Rigidbody2D rb = blade.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * throwSpeed, ForceMode2D.Impulse);

        // 4. 방향 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        blade.transform.rotation =
            Quaternion.Euler(0, 0, angle);

        Debug.Log("반인호 발사!");
        lastUsedTime = Time.time;
    }
}
