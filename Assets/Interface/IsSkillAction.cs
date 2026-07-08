using UnityEngine;

public interface ISkillAction
{
    void Init(CharacterData data, SkillData skillData);
    bool CanUse();
    bool TryUse(GameObject user, Vector2 dir);
}
