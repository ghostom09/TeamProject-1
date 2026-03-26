using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DescriptionData", menuName = "Scriptable Objects/DescriptionData")]
public class DescriptionData : ScriptableObject
{
    public CharName charName;
    public String description;
    public Sprite profile;
    public List<Sprite> skills;
}
