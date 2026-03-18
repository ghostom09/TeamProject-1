using UnityEngine;

public interface IBossSkillStrategy
{
    void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject hitArea, GameObject target);
    void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete);
    
    void EndAttack(GameObject hitArea, System.Action onComplete);
}
