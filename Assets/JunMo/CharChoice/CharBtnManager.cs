using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharBtnManager : MonoBehaviour
{
    [SerializeField] private List<CharJobData> characters;
    [SerializeField] private List<CharIconSetting> charBtn;
    [SerializeField] private List<Button> charBtns;
    [SerializeField] private List<GameObject> descriptions;
    void Start()
    {
        SetButton();
        SetCharData();
    }

    private void SetButton()
    {
        for (int i = 0; i < charBtn.Count; i++)
        {
            int index = i;
            charBtns[i].onClick.AddListener(() => OnClick(index));
        }
    }

    private void OnClick(int index)
    {
        OffImage();
        descriptions[index].SetActive(true);
    }

    private void OffImage()
    {
        foreach (GameObject obj in descriptions)
        {
            obj.SetActive(false);
        }
    }
    
    private void SetCharData()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            charBtn[i].Change(characters[i].profile, characters[i].charName);
        }
    }
}
