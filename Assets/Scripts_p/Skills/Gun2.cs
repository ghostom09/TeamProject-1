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
        
        SkillController.Instance.Gun2(user, true);
        player.StartCoroutine(TriggerAwakening(golden, executor, user));
        return true;
    }

    private IEnumerator TriggerAwakening(Gun1 golden, PlayerSkillExecutor executor, GameObject user)
    {
        Debug.Log("트리거 발동");

        // 기존 상태 저장
        bool originLock = golden.ignoreMoveLock;

        // 버프 적용
        golden.cooldownReductionRate += CooldownReduction;
        golden.ignoreMoveLock = true;
        executor.RefreshSkillCooldownUI();

        yield return new WaitForSeconds(AwakeningDuration);
        
        golden.cooldownReductionRate -= CooldownReduction;
        golden.ignoreMoveLock = originLock;
        executor.RefreshSkillCooldownUI();
        
        SkillController.Instance.Gun2(user, false);
        Debug.Log("트리거 종료");
    }
}
