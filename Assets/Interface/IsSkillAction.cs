using UnityEngine;

public interface ISkillAction
{
    bool CanUse();
    void TryUse(GameObject user, Vector2 dir);
}
