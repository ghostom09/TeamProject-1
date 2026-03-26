using UnityEngine;

public enum SceneName
{
    MainMenu,
    PlayerChoice,
    PlayerTest,
}
public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(Instance);
    }

    public void ChangeScene(SceneName sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName.ToString());
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
