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
    
    [SerializeField]private GameObject pop;
    [SerializeField]private GameObject stopGame;
    private InGameUIManager hudManager;
    
    private bool isPaused = false;
    private int playerLevel;
    private jobType jobType;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    

    private void OnEnable()
    {
        if (hudManager != null)
            hudManager.on_esc += OnEsc;
    }

    private void OnDisable()
    {
        if (hudManager != null)
            hudManager.on_esc -= OnEsc;
    }
    
    public void SetHUD(InGameUIManager hud)
    {
        if (hudManager != null)
            hudManager.on_esc -= OnEsc;
    
        hudManager = hud;
    
        if (hudManager != null)
            hudManager.on_esc += OnEsc;
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
        AudioListener.pause = true;
    }
    private void ResumeGame()
    {
        isPaused = false;
        stopGame.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
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

    public void UpdateInGameUI()
    {
        if (jobType.Sword == jobType)
        {
            SwordUI();
        }
        else GunUI();
    }

    private void SwordUI()
    {
        hudManager.UpdateProfile(swordIcon);
        hudManager.UpdateSkillTime(10,10,10);
    }

    private void GunUI()
    {
        hudManager.UpdateProfile(gunIcon);
        hudManager.UpdateSkillTime(10,10,10);
    }
}
