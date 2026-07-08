using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Description : MonoBehaviour
{
    private static readonly Vector2 CharacterImageSize = new Vector2(170.03f, 290.78f);

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
        int skillCount = Mathf.Min(skills.Count, data.skills.Count);
        for (int i = 0; i < skillCount; i++)
        {
            skills[i].sprite = data.skills[i];
        }

        profile.sprite = data.character;
        profile.preserveAspect = true;
        profile.rectTransform.sizeDelta = CharacterImageSize;
        charName.text = data.charName;
        description.text = data.description;
    }
}
