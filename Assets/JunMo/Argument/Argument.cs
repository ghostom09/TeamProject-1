using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public enum ArgumentKind
{
    None,
    Skill,
    Stat,
}
public class Argument : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI argumentName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Image icon;
    [SerializeField] private Image argumentPanel;

    public Action<int> on_Click;
    private int argumentID;
    public Vector3 normalScale = Vector3.one;
    private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.05f);

    private SkillArgumentData skillData;
    private StatArgumentData statData;
    public ArgumentKind kind = ArgumentKind.None;

    public void SetID(int value)
    {
        argumentID = value;
    }
    
    public void OnClick()
    {
        on_Click?.Invoke(argumentID);
    }
    
    public void SetSkill(SkillArgumentData data, int currentLevel)
    {
        Clear();
        if (data == null) 
            return;

        kind = ArgumentKind.Skill;
        skillData = data;
        statData = null;

        SetUI(
            data.itemName,
            data.description,
            data.icon,
            $"Lv {currentLevel}"
        );
    }
    public void SetStat(StatArgumentData data)
    {
        Clear();
        if (data == null) 
            return;
        
        kind = ArgumentKind.Stat;
        statData = data;
        skillData = null;
        SetUI(data.itemName, data.description, data.icon, "");
    }

    private void SetUI(string nameStr, string descStr, Sprite sprite, string levelStr)
    {
        argumentName.SetText(nameStr ?? "");
        description.SetText(descStr ?? "");
        level.SetText(levelStr ?? "");
        icon.sprite = sprite;
        icon.enabled = sprite;
    }
    
    private void Clear()
    {
        kind = ArgumentKind.None;
        skillData = null;
        statData = null;
        SetUI("", "", null, "");
    }
    
    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(duration));
        StartCoroutine(FadeCoroutine(duration));
    }
    private IEnumerator FadeCoroutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            icon.color = new Color(1f, 1f, 1f, alpha);
            argumentName.color = new Color(1f, 1f, 1f, alpha);
            description.color = new Color(1f, 1f, 1f, alpha);
            argumentPanel.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        icon.color = new Color(1f, 1f, 1f, 0f);
        argumentName.color = new Color(1f, 1f, 1f, 0f);
        description.color = new Color(1f, 1f, 1f, 0f);
        argumentPanel.color = new Color(1f, 1f, 1f, 0f);

        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = normalScale;
    }
}
