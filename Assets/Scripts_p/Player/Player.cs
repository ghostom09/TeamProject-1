using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;
    [SerializeField] private CharacterData character;
    [SerializeField] private PlayerAttack attacker;
    
    private void Start()
    {
        Init(character);
    }
    public void Init(CharacterData data)
    {
        playerSkillExecutor.Init(data.Skills);
        attacker.Init(data.JobType);
    }
    private void Update()
    {
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(0,Vector2.zero);
        }else if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(1,Vector2.zero);
        }
    }
}
