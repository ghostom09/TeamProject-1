using System.Collections;
using UnityEngine;

public class GunUlt : SkillBase
{
    private float gaugeDrainPerSecond = 6f;
    private float minGaugeToUse = 30f;

    public GunUlt() : base(0f)   // 즉발 데미지 없음
    {

    }
    
    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        // 이미 사용 중이면 해제
        if (player.isUsingUltimate)
        {
            player.StopUltimate();
            return;
        }

        // 게이지 부족하면 실행 안 함
        if (player.currentGauge < minGaugeToUse)
            return;

        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(MindWorldRoutine(user));
    }

    private IEnumerator MindWorldRoutine(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        PlayerMove playerMove = user.GetComponent<PlayerMove>();
        PlayerAttack attack = user.GetComponent<PlayerAttack>();
        GunNormalAttack gun = attack.GetNormalAttack() as GunNormalAttack;

        player.isUsingUltimate = true;
        player.isInvincible = true;
        playerMove.SetMoveLock(true);
        gun.SetEnhancedMode(true);

        // 강화 사격 모드 ON

        while (player.UseGauge(gaugeDrainPerSecond))
        {
            
            yield return new WaitForSeconds(1f);

            // 중간에 다시 누르면 종료
            if (!player.isUsingUltimate)
                break;
        }

        // 종료 처리
        player.isInvincible = false;
        playerMove.SetMoveLock(false);
        player.isUsingUltimate = false;

 ;
        gun.SetEnhancedMode(false);
    }
}
