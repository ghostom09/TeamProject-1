using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField]private Sprite gunIcon;
    [SerializeField]private Sprite swordIcon;
    [SerializeField] private List<Sprite> swordSkills;
    [SerializeField] private List<Sprite> gunSkills;
    
    [SerializeField]private GameObject pop;
    [SerializeField]private GameObject stopGame;
    [SerializeField]private Button restart;
    [SerializeField]private Button toLobby;
    private InGameUIManager hudManager;
    private SkillData[] currentSkills;
    private float[] currentCooldowns;
    
    private bool isPaused = false;
    private int playerLevel;
    private CharacterData currentCharacter;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (hudManager != null)
        {
            hudManager.on_esc += OnEsc;
            UpdateCurrentSkillCooldownUI();
        }
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
        BindInGameUI();
    
        if (hudManager != null)
        {
            hudManager.on_esc += OnEsc;
            UpdateCurrentSkillCooldownUI();
        }
    }

    private void BindInGameUI()
    {
        stopGame = FindSceneObject("StopGameBackground");
        restart = FindSceneButton("Restart");
        toLobby = FindSceneButton("Lobby");
        
        if (restart != null)
        {
            restart.onClick.RemoveListener(RestartGame);
            restart.onClick.AddListener(RestartGame);
        }

        if (toLobby != null)
        {
            toLobby.onClick.RemoveListener(QuitInGame);
            toLobby.onClick.AddListener(QuitInGame);
        }
    }

    private GameObject FindSceneObject(string objectName)
    {
        UnityEngine.SceneManagement.Scene activeScene =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.scene == activeScene && obj.name == objectName)
                return obj;
        }

        return null;
    }

    private Button FindSceneButton(string objectName)
    {
        GameObject obj = FindSceneObject(objectName);
        return obj != null ? obj.GetComponent<Button>() : null;
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
        if (stopGame != null)
            stopGame.SetActive(true);
        Time.timeScale = 0;
        AudioListener.pause = true;
    }
    private void ResumeGame()
    {
        isPaused = false;
        if (stopGame != null)
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
        ResumeGame();
        SceneManager.Instance.ChangeScene(SceneName.MainMenu);
    }

    private void RestartGame()
    {
        ResumeGame();
        hudManager = null;
        string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (SceneManager.Instance != null)
            SceneManager.Instance.ChangeScene(activeSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(activeSceneName);
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
        CharacterData selectedCharacter = currentCharacter;

        if (selectedCharacter == null && CharDataManager.Instance != null)
            selectedCharacter = CharDataManager.Instance.data;

        if (selectedCharacter == null || hudManager == null)
            return;

        if (selectedCharacter.JobType == jobType.Sword)
        {
            SwordUI();
        }
        else GunUI();
    }

    public void SetCharacter(CharacterData character)
    {
        currentCharacter = character;
        UpdateInGameUI();
    }

    private void SwordUI()
    {
        hudManager.UpdateProfile(swordIcon);
        hudManager.UpdateSkillIcons(swordSkills);
    }

    private void GunUI()
    {
        hudManager.UpdateProfile(gunIcon);
        hudManager.UpdateSkillIcons(gunSkills);
    }

    public void SetSkillCooldowns(SkillData[] skills)
    {
        currentSkills = skills;
        currentCooldowns = null;
        hudManager?.UpdateSkillTime(skills);
    }

    public void SetSkillCooldowns(float[] cooldowns)
    {
        currentCooldowns = cooldowns;
        hudManager?.UpdateSkillTime(currentCooldowns);
    }

    private void UpdateCurrentSkillCooldownUI()
    {
        if (currentCooldowns != null)
            hudManager.UpdateSkillTime(currentCooldowns);
        else
            hudManager.UpdateSkillTime(currentSkills);
    }

    public void UpdateSkillTimer(int index)
    {
        hudManager?.UpdateSkillTimer(index);
    }

    public void ShowMagicianUltimateEffect(float duration)
    {
        hudManager?.ShowMagicianUltimateEffect(duration);
    }

    public void HideMagicianUltimateEffect()
    {
        hudManager?.HideMagicianUltimateEffect();
    }
}
