using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField]private Sprite gunIcon;
    [SerializeField]private Sprite swordIcon;
    [SerializeField] private List<Sprite> swordSkills;
    [SerializeField] private List<Sprite> gunSkills;
    
    [SerializeField]private InGameUIManager hudManager;
    [SerializeField]private GameObject pop;
    [SerializeField]private GameObject stopGame;
    
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }
    

    private void OnEnable()
    {
        hudManager.on_esc += OnEsc;
    }

    private void OnDisable()
    {
        hudManager.on_esc -= OnEsc;
    }


    private void OnEsc()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        stopGame.SetActive(true);
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        isPaused = false;
        stopGame.SetActive(false);
        Time.timeScale = 1;
    }

    public void CloseStopUI()
    {
        ResumeGame();
    }


    public void QuitInGame()
    {
        SceneManager.Instance.ChangeScene(SceneName.MainMenu);
    }

    public void Open(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void Close(GameObject obj)
    { 
        obj.SetActive(false);
    }
}
