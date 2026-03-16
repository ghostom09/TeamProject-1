using UnityEngine;

public class MagicianNormalAttack : IBossSkillStrategy
{
        
    public void Init(BossSkills data, BossAttack bossAttack, GameObject hitBox, GameObject hitArea)
    {
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
    }
    
    public void EndAttack(GameObject hitBox, GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
