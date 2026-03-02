using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public Slider masterVolumeSlider;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;

    GameSettings settings;

    void Start()
    {
        settings = SettingManager.Instance.currentSettings;

        InitResolutionDropdown();
        LoadToUI();
    }
    

    void InitResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        var resolutions = SettingManager.Instance.GetResolutions();

        System.Collections.Generic.List<string> options = new();

        foreach (var res in resolutions)
        {
            options.Add(res.width + " x " + res.height);
        }

        resolutionDropdown.AddOptions(options);
    }

    void LoadToUI()
    {
        masterVolumeSlider.value = settings.masterVolume;

        resolutionDropdown.value = settings.resolutionIndex;
        fullscreenToggle.isOn = settings.fullScreen;
        qualityDropdown.value = settings.qualityLevel;
    }

    public void OnMasterVolumeChanged(float value)
    {
        settings.masterVolume = value;
    }

    public void OnResolutionChanged(int index)
    {
        settings.resolutionIndex = index;
    }

    public void OnFullscreenChanged(bool value)
    {
        settings.fullScreen = value;
    }

    public void OnQualityChanged(int index)
    {
        settings.qualityLevel = index;
    }

    public void OnApply()
    {
        SettingManager.Instance.ApplySettings();
    }

    public void OnResetDefault()
    {
        SettingManager.Instance.currentSettings = new GameSettings();
        settings = SettingManager.Instance.currentSettings;

        LoadToUI();
    }
}
