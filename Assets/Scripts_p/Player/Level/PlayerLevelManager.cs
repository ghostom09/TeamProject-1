using System;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExp { get; private set; }

    [SerializeField] private ExperienceTable expTable;

    public void AddExp(int amount)
    {
        CurrentExp += amount;
        
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (CurrentExp >= expTable.GetRequiredExp(CurrentLevel))
        {
            CurrentExp -= expTable.GetRequiredExp(CurrentLevel);
            LevelUp();
            GetComponent<IPlayerStatUp>()?.StatUp();
        }
    }

    private void LevelUp()
    {
        CurrentLevel++;
        OnLevelUp?.Invoke(CurrentLevel);
    }

    public static event Action<int> OnLevelUp;
}
