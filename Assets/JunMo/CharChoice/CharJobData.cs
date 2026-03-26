using UnityEngine;

public enum CharName
{
    Sword,
    Gun
}

[CreateAssetMenu(fileName = "CharJobData", menuName = "Scriptable Objects/CharJobData")]
public class CharJobData : ScriptableObject
{
    public Sprite profile;
    public CharName charName;
}
