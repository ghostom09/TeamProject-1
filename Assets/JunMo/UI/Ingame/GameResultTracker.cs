using UnityEngine;

public class GameResultTracker : MonoBehaviour
{
    private const string BestTimeKey = "GameOver_BestSurvivalTime";
    private const string DefaultDeathReason = "Defeated by enemy attack";

    public static GameResultTracker Instance { get; private set; }

    [SerializeField] private string deathReason = DefaultDeathReason;

    private float survivalTime;
    private int killCount;
    private bool isTracking = true;

    public float SurvivalTime => survivalTime;
    public int KillCount => killCount;
    public string DeathReason => string.IsNullOrWhiteSpace(deathReason) ? DefaultDeathReason : deathReason;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!isTracking)
            return;

        survivalTime += Time.deltaTime;
    }

    public void RegisterKill()
    {
        killCount++;
    }

    public void SetDeathReason(string reason)
    {
        if (!string.IsNullOrWhiteSpace(reason))
            deathReason = reason;
    }

    public ResultData BuildResult(PlayerLevelManager levelManager, DifficultyLevelManager difficultyManager, ArgumentDataManager argumentManager)
    {
        isTracking = false;

        // Best record is persisted independently from the UI so the result panel stays display-only.
        float previousBest = PlayerPrefs.GetFloat(BestTimeKey, 0f);
        bool isNewRecord = survivalTime > previousBest;
        if (isNewRecord)
        {
            PlayerPrefs.SetFloat(BestTimeKey, survivalTime);
            PlayerPrefs.Save();
        }

        return new ResultData
        {
            SurvivalTime = survivalTime,
            Difficulty = difficultyManager != null ? difficultyManager.CurrentDifficulty : 0,
            KillCount = killCount,
            Level = levelManager != null ? levelManager.CurrentLevel : 1,
            Exp = levelManager != null ? levelManager.TotalExp : 0,
            DeathReason = DeathReason,
            Augments = argumentManager != null ? argumentManager.GetOwnedAugments() : new System.Collections.Generic.List<AugmentData>(),
            IsNewRecord = isNewRecord,
            BestTime = isNewRecord ? survivalTime : previousBest
        };
    }
}
