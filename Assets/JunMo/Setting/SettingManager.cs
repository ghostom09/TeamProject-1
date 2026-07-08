using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public enum VolumeType
{
    MasterVolume,
    BGMVolume,
    SFXVolume,
}
public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    public AudioMixer audioMixer;
    public GameSettings currentSettings;

    Resolution[] resolutions;
    string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Application.persistentDataPath + "/settings.json";
        resolutions = Screen.resolutions;

        LoadSettings();

        if (currentSettings.resolutionIndex == -1)
            SetDefaultResolution();

        ApplySettings();
    }

    public int GetDefaultResolutionIndex()
    {
        float targetRatio = (float)Screen.currentResolution.width / Screen.currentResolution.height;

        int bestIndex = 0;
        int bestPixels = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            float ratio = (float)resolutions[i].width / resolutions[i].height;

            if (Mathf.Abs(ratio - targetRatio) < 0.03f)
            {
                int pixels = resolutions[i].width * resolutions[i].height;

                if (pixels > bestPixels)
                {
                    bestPixels = pixels;
                    bestIndex = i;
                }
            }
        }

        return bestIndex;
    }

    void SetDefaultResolution()
    {
        currentSettings.resolutionIndex = GetDefaultResolutionIndex();
    }

    public void ApplySettings()
    {
        ApplyAudio();
        ApplyGraphics();
        SaveSettings();
    }

    public void ApplyAudio()
    {
        SetMixerVolume(VolumeType.MasterVolume.ToString(), currentSettings.masterVolume);
        SetMixerVolume(VolumeType.BGMVolume.ToString(), currentSettings.bgmVolume);
        SetMixerVolume(VolumeType.SFXVolume.ToString(), currentSettings.sfxVolume);
    }

    public void ApplyGraphics()
    {
        int index = Mathf.Clamp(currentSettings.resolutionIndex, 0, resolutions.Length - 1);
        Resolution res = resolutions[index];

        Screen.SetResolution(res.width, res.height, currentSettings.fullScreen);
        QualitySettings.SetQualityLevel(currentSettings.qualityLevel);
    }

    void SetMixerVolume(string paramName, float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        audioMixer.SetFloat(paramName, dB);
    }

    public Resolution[] GetResolutions() => resolutions;

    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(currentSettings, true);
            File.WriteAllText(savePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"설정 저장 실패: {e.Message}");
        }
    }

    public void LoadSettings()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                currentSettings = JsonUtility.FromJson<GameSettings>(json);
            }
            else
            {
                currentSettings = new GameSettings();
            }
        }
        catch
        {
            currentSettings = new GameSettings();
        }
    }
}