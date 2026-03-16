using System.Collections;
using UnityEngine;

public class Gun2 : SkillBase
{
    private const float AwakeningDuration = 8f;
    private const float CooldownReduction = 0.5f;

    protected override bool Execute(GameObject user, Vector2 dir)
    {
        Player player = user.GetComponent<Player>();

        if (player.isUsingUltimate)
            return false;

        PlayerSkillExecutor executor = player.GetComponent<PlayerSkillExecutor>();

        if (executor == null)
        {
            Debug.LogError("PlayerSkillExecutor 없음!");
            return false;
        }

        Gun1 golden = executor.GetSkill(SkillType.Gun1) as Gun1;

        if (golden == null)
        {
            Debug.LogError("Gun1 스킬을 찾을 수 없음");
            return false;
        }

        player.StartCoroutine(TriggerAwakening(golden));
        return true;
    }

    private IEnumerator TriggerAwakening(Gun1 golden)
    {
        Debug.Log("트리거 발동");

        // 기존 상태 저장
        bool originLock = golden.ignoreMoveLock;

        // 버프 적용
        golden.cooldownReductionRate += 0.5f;
        golden.ignoreMoveLock = true;

        yield return new WaitForSeconds(AwakeningDuration);
        
        golden.cooldownReductionRate -= 0.5f;
        golden.ignoreMoveLock = originLock;

        Debug.Log("트리거 종료");
    }
}