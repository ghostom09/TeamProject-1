using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Description : MonoBehaviour
{
    [SerializeField] private Image profile;
    [SerializeField] private List<Image> skills;
    [SerializeField] private TextMeshProUGUI charName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private DescriptionData data;
    private void OnEnable()
    {
        SetSetting();
    }

    private void SetSetting()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].sprite = data.skills[i];
        }
        profile.sprite = data.profile;
        charName.text = data.charName.ToString();
        description.text = data.description;
    }
}
