using System;

[Serializable]
public class GameSettings
{
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public int resolutionIndex = 0;
    public bool fullScreen = true;
    public int qualityLevel = 2;
}