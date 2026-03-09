using UnityEngine;

public interface ISkillAction
{
    void Init(CharacterData data, SkillData skillData);
    bool CanUse();
    void TryUse(GameObject user, Vector2 dir);
}
