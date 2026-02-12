using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum ArgumentKind
{
    None,
    Skill,
    Stat,
}
public class Argument : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI argumentName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Image icon;
    [SerializeField]private Image canvas;

    public Action<int> on_Click;
    private int argumentID;

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
    
    public void SetSkill(SkillArgumentData data)
    {
        Clear();
        if (data == null) 
            return;
        
        kind = ArgumentKind.Skill;
        skillData = data;
        statData = null;
        SetUI(data.itemName, data.description, data.icon, data.level.ToString());
    }
    public void SetStat(StatArgumentData data)
    {
        Clear();
        if (data == null) 
            return;
        
        kind = ArgumentKind.Skill;
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
        icon.enabled = sprite != null;
    }
    
    private void Clear()
    {
        kind = ArgumentKind.None;
        skillData = null;
        statData = null;
        SetUI("", "", null, "");
    }
    
    public SkillArgumentData GetSkillData() => kind == ArgumentKind.Skill ? skillData : null;
    public StatArgumentData  GetStatData()  => kind == ArgumentKind.Stat  ? statData  : null;
    
    public void FadeOut(float duration)
    {
        StopAllCoroutines();
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

            canvas.color = new Color(1f, 1f, 1f, alpha);
            icon.color = new Color(1f, 1f, 1f, alpha);
            argumentName.color = new Color(1f, 1f, 1f, alpha);
            description.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        canvas.color = new Color(1f, 1f, 1f, 0f);
        icon.color = new Color(1f, 1f, 1f, 0f);
        argumentName.color = new Color(1f, 1f, 1f, 0f);
        description.color = new Color(1f, 1f, 1f, 0f);

        Destroy(gameObject);
    }
}
