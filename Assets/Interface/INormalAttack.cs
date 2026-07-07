using UnityEngine;

public interface INormalAttack
{
    int LastAttackComboIndex { get; }
    bool TryAttack(GameObject user, Vector2 dir);
    void Init(CharacterData data);
}
