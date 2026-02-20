using UnityEngine;

public enum JobType
{
    Gun,
    Sword,
}
public enum SkillKind
{
    SwordMove,
    SwordCC,
    SwordUltra,
    GunShot,
    GunTrigger,
    GunUltra,
}

[CreateAssetMenu(fileName = "SkillArgumentData", menuName = "Scriptable Objects/SkillArgumentData")]
public class SkillArgumentData : ArgumentData
{
    public SkillKind skillKind;
    public JobType jobType;
    
    public int maxLevel = 5;
    public int level;
    public int skillTime;
    
    public float[] levelValues;          // 레벨 1~5일 때의 실제 값 배열 (또는 증가량 배열)
}