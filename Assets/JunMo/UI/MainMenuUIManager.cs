using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject ExitUI;
    
    public void StartGame()
    {
        SceneManager.Instance.ChangeScene(SceneName.PlayerChoice);
    }

    public void SettingOn()
    {
        UIManager.Instance.Open(SettingUI);
    }

    public void SettingOff()
    {
        UIManager.Instance.Close(SettingUI);
    }

    public void QuitGame()
    {
        UIManager.Instance.Open(ExitUI);
    }

    public void QuitGameNo()
    {
        UIManager.Instance.Close(ExitUI);
    }

    public void QuitGameYes()
    {
        SceneManager.Instance.QuitGame();
    }
}
