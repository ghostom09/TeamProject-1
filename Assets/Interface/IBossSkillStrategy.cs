using UnityEngine;

public interface IBossSkillStrategy
{
    void Init(BossSkills data, BossAttack bossAttack, GameObject hitBox, GameObject hitArea);
    void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete);
    
    void EndAttack(GameObject hitBox, GameObject hitArea, System.Action onComplete);
}
