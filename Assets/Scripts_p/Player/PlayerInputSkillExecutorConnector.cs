using UnityEngine;

public class PlayerInputSkillExecutorConnector : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor;

    public void Connect()
    {
        playerInput.onSkills += playerSkillExecutor.GetDirection;
    }

    //테스트
    private void Start()
    {
        Connect();
    }
}
