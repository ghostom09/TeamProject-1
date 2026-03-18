using UnityEngine;

public class MagicianLongAttack : IBossSkillStrategy
{
    public void Init(GameObject boss, BossSkills data, BossAttack bossAttack, GameObject hitArea, GameObject target)
    {
    }

    public void TryAttack(GameObject boss, GameObject target, Vector2 direction, System.Action onComplete)
    {
    }
    
    public void EndAttack(GameObject hitArea, System.Action onComplete)
    {
        onComplete?.Invoke();
    }
}
