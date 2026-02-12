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
        playerSkillExecutor.Init(data.Skills, data);
        attacker.Init(data);
    }
    private void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(0,dir);
        }else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            playerSkillExecutor.UseSkill(1,dir);
        }
    }
}
