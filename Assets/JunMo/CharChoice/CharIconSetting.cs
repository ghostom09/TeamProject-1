using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharIconSetting : MonoBehaviour
{
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI charName;
    public void Change(DescriptionData descriptionData)
    {
        this.profile.sprite = descriptionData.profile;
        this.charName.SetText(descriptionData.charName);
    }
}
