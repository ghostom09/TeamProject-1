using System.Collections.Generic;
using UnityEngine;

public enum BossType
{
    None,
    warrior,
    magician
}

[CreateAssetMenu(fileName = "BossStats")]

public class BossStats : ScriptableObject
{
    public BossType bossType;
    public float health;
    public float speed;
    public float jumpForce;
    public int exp;
    
    public float skillInterval;
    
    public List<BossSkills> skills;
}
