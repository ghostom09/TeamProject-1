using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DescriptionData", menuName = "Scriptable Objects/DescriptionData")]
public class DescriptionData : ScriptableObject
{
    public String description;
    public Sprite profile;
    public Sprite character;
    public string charName;
    public List<Sprite> skills;
}
