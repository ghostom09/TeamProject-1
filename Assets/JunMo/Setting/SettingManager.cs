using UnityEngine;
using System.IO;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;
    
    public GameSettings currentSettings;
    
    Resolution[] resolutions;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        resolutions = Screen.resolutions;

        ApplySettings();
    }


    public void ApplySettings()
    {
        ApplyAudio();
        ApplyGraphics();
    }
    
    public void ApplyAudio()
    {
        AudioListener.volume = currentSettings.masterVolume;
    }

    public void ApplyGraphics()
    {
        Resolution res = resolutions[currentSettings.resolutionIndex];

        Screen.SetResolution(
            res.width,
            res.height,
            currentSettings.fullScreen
        );

        QualitySettings.SetQualityLevel(currentSettings.qualityLevel);
    }

    public Resolution[] GetResolutions()
    {
        return resolutions;
    }
}
