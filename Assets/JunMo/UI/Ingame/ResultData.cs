using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AugmentData
{
    public string Name;
    public Sprite Icon;
}

[Serializable]
public class ResultData
{
    public float SurvivalTime;
    public int Difficulty;
    public int KillCount;
    public int Level;
    public int Exp;
    public string DeathReason;
    public List<AugmentData> Augments = new List<AugmentData>();
    public bool IsNewRecord;
    public float BestTime;
}
