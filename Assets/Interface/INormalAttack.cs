using UnityEngine;

public interface INormalAttack
{
    bool TryAttack(GameObject user, Vector2 dir);
    void Init(CharacterData data);
}
