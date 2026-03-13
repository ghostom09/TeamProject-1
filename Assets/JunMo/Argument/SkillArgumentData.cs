using UnityEngine;

[CreateAssetMenu(fileName = "SkillArgumentData", menuName = "Scriptable Objects/SkillArgumentData")]
public class SkillArgumentData : ArgumentData
{
    public int maxLevel = 5;
    public int level = 1;
    public SkillType skillType;
    public float[] levelValues;
}