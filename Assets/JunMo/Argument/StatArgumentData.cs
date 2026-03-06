using UnityEngine;

public enum StatType
{
    Health,
    Attack,
    AttackSpeed,
    Speed,
    Range,
    Heal
}

public enum ModifierType //아직 안씀 확장용
{
    FlatAdd = 0,
    PercentAdd = 10,
    PercentMultiply = 20,
    Override = 100,
}

[CreateAssetMenu(fileName = "StatArgumentData", menuName = "Scriptable Objects/StatArgumentData")]
public class StatArgumentData : ArgumentData
{
    public StatType statType;
    public ModifierType modifierType;

    public float value;
}
