using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;

    public void Init(CharacterData data)
    {
        playerSkillExecutor.Init(data.Skills);
    }
}
