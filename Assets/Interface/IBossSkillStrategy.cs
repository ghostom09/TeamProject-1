using UnityEngine;

public interface IBossSkillStrategy
{
    void Init(BossSkills data, BossAttack bossAttack);
    void TryAttack(GameObject boss, Transform target, Vector2 direction);
}
