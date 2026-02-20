using System.Collections;
using UnityEngine;

public class Gun2 : SkillBase
{
    public Gun2() { }

    protected override void Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (player.isUsingUltimate)
            return;
        var executor = user.GetComponent<PlayerSkillExecutor>();
        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(TriggerAwakening(executor));
    }

    private IEnumerator TriggerAwakening(PlayerSkillExecutor executor)
    {
        Debug.Log("트리거 발동");

        var golden = executor.GetSkill(SkillType.Gun1) as Gun1;

        // 1. 기존 상태 저장
        float originBonus = golden.Cooldown;
        bool originLock = golden.ignoreMoveLock;

        // 2. 버프 적용
        golden.AddCooldownBonus(-golden.Cooldown * 0.5f);
        golden.ignoreMoveLock = true;

        yield return new WaitForSeconds(8f);

        // 3. 원상복구
        golden.ResetBonus();
        golden.ignoreMoveLock = originLock;

        Debug.Log("트리거 종료");
    }
}
