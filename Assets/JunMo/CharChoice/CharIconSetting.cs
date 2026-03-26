using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharIconSetting : MonoBehaviour
{
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI charName;
    public void Change(Sprite profile, CharName charName)
    {
        this.profile.sprite = profile;
        this.charName.SetText(charName.ToString());
    }
}
