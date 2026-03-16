using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    [Header("오디오")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("그래픽")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    public TMP_Dropdown qualityDropdown;

    [Header("버튼")]
    public Button applyButton;
    public Button cancelButton;
    public Button resetButton;

    GameSettings tempSettings;

    void OnEnable()
    {
        // 설정창 열릴 때 현재 설정값 복사해서 임시 저장
        tempSettings = JsonUtility.FromJson<GameSettings>(
            JsonUtility.ToJson(SettingManager.Instance.currentSettings)
        );

        InitUI();
    }

    void InitUI()
    {
        RemoveListeners();
        
        var settings = tempSettings;
        var resolutions = SettingManager.Instance.GetResolutions();

        // 슬라이더
        masterSlider.value = settings.masterVolume;
        bgmSlider.value = settings.bgmVolume;
        sfxSlider.value = settings.sfxVolume;

        // 전체화면
        fullScreenToggle.isOn = settings.fullScreen;

        // 해상도 드롭다운
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = Mathf.Clamp(settings.resolutionIndex, 0, resolutions.Length - 1);
        resolutionDropdown.RefreshShownValue();

        // 품질 드롭다운 (Unity 기본 퀄리티 레벨 이름 사용)
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        qualityDropdown.value = settings.qualityLevel;
        qualityDropdown.RefreshShownValue();
        
        RegisterListeners();
    }
    
    void RegisterListeners()
    {
        masterSlider.onValueChanged.AddListener(v => tempSettings.masterVolume = v);
        bgmSlider.onValueChanged.AddListener(v => tempSettings.bgmVolume = v);
        sfxSlider.onValueChanged.AddListener(v => tempSettings.sfxVolume = v);
        fullScreenToggle.onValueChanged.AddListener(v => tempSettings.fullScreen = v);
        resolutionDropdown.onValueChanged.AddListener(v => tempSettings.resolutionIndex = v);
        qualityDropdown.onValueChanged.AddListener(v => tempSettings.qualityLevel = v);
        applyButton.onClick.AddListener(OnApply);
        cancelButton.onClick.AddListener(OnCancel);
        resetButton.onClick.AddListener(OnReset);
    }

    void RemoveListeners()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        fullScreenToggle.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        qualityDropdown.onValueChanged.RemoveAllListeners();
        applyButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
    }

    void OnApply()
    {
        SettingManager.Instance.currentSettings = tempSettings;
        SettingManager.Instance.ApplySettings();
    }

    void OnCancel()
    {
        gameObject.SetActive(false);
    }
    
    // SettingUI.cs
    void OnReset()
    {
        tempSettings = new GameSettings();
        tempSettings.resolutionIndex = SettingManager.Instance.GetDefaultResolutionIndex();
        InitUI();
    }

    void OnDisable()
    {
        RemoveListeners();
    }
}