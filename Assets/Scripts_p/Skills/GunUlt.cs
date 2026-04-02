using System.Collections;
using UnityEngine;

public class GunUlt : SkillBase
{
    private const float GaugeDrainPerSecond = 6f;
    private const float MinGaugeToUse = 30f;

    protected override bool Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();
        
        if (player.isUsingUltimate)
        {
            player.StopUltimate();
            return false;
        }

        // 게이지 부족
        if (player.currentGauge < MinGaugeToUse)
            return false;

        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        PlayerAttack attack = player.GetComponent<PlayerAttack>();

        GunNormalAttack gun = attack.GetNormalAttack() as GunNormalAttack;
        SkillController.Instance.GunUltra();

        player.StartCoroutine(MindWorldRoutine(player, playerMove, gun));

        return true;
    }

    private IEnumerator MindWorldRoutine(
        Player player,
        PlayerMove playerMove,
        GunNormalAttack gun
    )
    {
        player.isUsingUltimate = true;
        player.isInvincible = true;

        playerMove.SetMoveLock(MoveLockType.HorizontalOnly);
        gun.SetEnhancedMode(true);

        while (true)
        {
            if (!player.UseGauge(GaugeDrainPerSecond))
                break;
            
            yield return new WaitForSeconds(1f);

            if (!player.isUsingUltimate)
                break;
        }

        // 종료 처리
        player.isInvincible = false;
        playerMove.SetMoveLock(MoveLockType.None);
        player.isUsingUltimate = false;

        gun.SetEnhancedMode(false);
    }
}