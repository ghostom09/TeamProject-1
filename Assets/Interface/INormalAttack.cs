using UnityEngine;

public interface INormalAttack
{
    bool TryAttack(GameObject user, Vector2 dir, GameObject hitBox);
    void EndAttack(GameObject user, GameObject hitBox);
    void Init(CharacterData data);
}
