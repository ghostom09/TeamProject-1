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
    private const float PanelAlpha = 0.96f;
    private static readonly Color SkillAccent = new Color(1f, 0.72f, 0.28f, 1f);
    private static readonly Color StatAccent = new Color(0.34f, 0.9f, 0.82f, 1f);
    private static readonly Color EmptyAccent = new Color(0.62f, 0.72f, 0.9f, 1f);
    private static readonly Color PanelColor = new Color(0.055f, 0.07f, 0.105f, PanelAlpha);
    private static readonly Color HoverPanelColor = new Color(0.085f, 0.105f, 0.15f, PanelAlpha);
    private static readonly Color NameColor = new Color(0.98f, 0.96f, 0.88f, 1f);
    private static readonly Color DescriptionColor = new Color(0.76f, 0.83f, 0.9f, 1f);
    private static readonly Color LevelColor = new Color(0.08f, 0.08f, 0.1f, 1f);

    [SerializeField] private TextMeshProUGUI argumentName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Image icon;
    [SerializeField] private Image argumentPanel;
    [SerializeField] private Image accentBar;
    [SerializeField] private Image iconBackplate;
    [SerializeField] private Image bottomLine;

    public Action<int> on_Click;
    private int argumentID;
    public Vector3 normalScale = Vector3.one;
    private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.05f);
    private bool isHovering;

    private Color panelBaseColor;
    private Color accentBaseColor;
    private Color iconBackplateBaseColor;
    private Color bottomLineBaseColor;
    private Color iconBaseColor;
    private Color nameBaseColor;
    private Color descriptionBaseColor;
    private Color levelBaseColor;

    private SkillArgumentData skillData;
    private StatArgumentData statData;
    public ArgumentKind kind = ArgumentKind.None;

    private void Awake()
    {
        SetupVisuals();
        ApplyStyle(ArgumentKind.None);
    }

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
        ApplyStyle(kind);
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
        ApplyStyle(kind);
    }

    private void SetUI(string nameStr, string descStr, Sprite sprite, string levelStr)
    {
        argumentName.SetText(nameStr ?? "");
        description.SetText(descStr ?? "");
        level.SetText(levelStr ?? "");
        icon.sprite = sprite;
        icon.enabled = sprite;
        icon.preserveAspect = true;
    }
    
    private void Clear()
    {
        kind = ArgumentKind.None;
        skillData = null;
        statData = null;
        SetUI("", "", null, "");
        ApplyStyle(ArgumentKind.None);
    }
    
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
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            SetFadeAlpha(alpha);

            yield return null;
        }

        SetFadeAlpha(0f);

        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        transform.localScale = hoverScale;
        ApplyPanelState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        transform.localScale = normalScale;
        ApplyPanelState();
    }

    private void SetupVisuals()
    {
        if (argumentPanel == null)
            argumentPanel = GetComponent<Image>();

        if (argumentPanel != null)
            argumentPanel.raycastTarget = true;

        accentBar = accentBar != null
            ? accentBar
            : CreateDecorImage("AccentBar", new Vector2(0f, 98f), new Vector2(118f, 5f), 0);

        iconBackplate = iconBackplate != null
            ? iconBackplate
            : CreateDecorImage("IconBackplate", new Vector2(0f, 45f), new Vector2(104f, 92f), 0);

        bottomLine = bottomLine != null
            ? bottomLine
            : CreateDecorImage("BottomLine", new Vector2(0f, -98f), new Vector2(94f, 2f), 0);

        ConfigureRect(icon, new Vector2(0f, 47f), new Vector2(88f, 82f));
        ConfigureRect(argumentName, new Vector2(0f, -23f), new Vector2(120f, 42f));
        ConfigureRect(description, new Vector2(0f, -69f), new Vector2(112f, 66f));
        ConfigureRect(level, new Vector2(42f, 82f), new Vector2(48f, 22f));

        ConfigureText(argumentName, 18f, 13f, 20f, FontStyles.Bold, NameColor);
        ConfigureText(description, 12.5f, 10f, 14f, FontStyles.Normal, DescriptionColor);
        ConfigureText(level, 12f, 10f, 13f, FontStyles.Bold, LevelColor);

        ConfigureShadow(argumentName, new Color(0f, 0f, 0f, 0.65f), new Vector2(0f, -1.5f));
        ConfigureShadow(description, new Color(0f, 0f, 0f, 0.55f), new Vector2(0f, -1f));

        if (icon != null)
            icon.raycastTarget = false;

        ConfigurePanelEffects(EmptyAccent);
        ConfigureButtonColors();
    }

    private Image CreateDecorImage(string objectName, Vector2 anchoredPosition, Vector2 size, int siblingIndex)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.layer = gameObject.layer;
        obj.transform.SetParent(transform, false);
        obj.transform.SetSiblingIndex(siblingIndex);

        Image image = obj.GetComponent<Image>();
        image.raycastTarget = false;

        ConfigureRect(image, anchoredPosition, size);
        return image;
    }

    private static void ConfigureRect(Graphic graphic, Vector2 anchoredPosition, Vector2 size)
    {
        if (graphic == null)
            return;

        ConfigureRect(graphic.rectTransform, anchoredPosition, size);
        graphic.raycastTarget = false;
    }

    private static void ConfigureRect(RectTransform rect, Vector2 anchoredPosition, Vector2 size)
    {
        if (rect == null)
            return;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }

    private static void ConfigureText(
        TextMeshProUGUI text,
        float fontSize,
        float minSize,
        float maxSize,
        FontStyles style,
        Color color)
    {
        if (text == null)
            return;

        text.color = color;
        text.fontSize = fontSize;
        text.fontSizeMin = minSize;
        text.fontSizeMax = maxSize;
        text.enableAutoSizing = true;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = style;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.characterSpacing = 0f;
    }

    private static void ConfigureShadow(Graphic graphic, Color color, Vector2 distance)
    {
        if (graphic == null)
            return;

        Shadow shadow = graphic.GetComponent<Shadow>();
        if (shadow == null)
            shadow = graphic.gameObject.AddComponent<Shadow>();

        shadow.effectColor = color;
        shadow.effectDistance = distance;
        shadow.useGraphicAlpha = true;
    }

    private void ApplyStyle(ArgumentKind styleKind)
    {
        Color accent = GetAccentColor(styleKind);

        panelBaseColor = PanelColor;
        accentBaseColor = accent;
        iconBackplateBaseColor = new Color(accent.r, accent.g, accent.b, 0.16f);
        bottomLineBaseColor = new Color(accent.r, accent.g, accent.b, 0.45f);
        iconBaseColor = Color.white;
        nameBaseColor = NameColor;
        descriptionBaseColor = DescriptionColor;
        levelBaseColor = LevelColor;

        if (argumentPanel != null)
            argumentPanel.color = panelBaseColor;

        if (accentBar != null)
            accentBar.color = accentBaseColor;

        if (iconBackplate != null)
            iconBackplate.color = iconBackplateBaseColor;

        if (bottomLine != null)
            bottomLine.color = bottomLineBaseColor;

        if (icon != null)
            icon.color = iconBaseColor;

        if (argumentName != null)
            argumentName.color = nameBaseColor;

        if (description != null)
            description.color = descriptionBaseColor;

        if (level != null)
            level.color = levelBaseColor;

        ConfigurePanelEffects(accent);
        ConfigureButtonColors();
        ApplyPanelState();
    }

    private Color GetAccentColor(ArgumentKind styleKind)
    {
        switch (styleKind)
        {
            case ArgumentKind.Skill:
                return SkillAccent;
            case ArgumentKind.Stat:
                return StatAccent;
            default:
                return EmptyAccent;
        }
    }

    private void ConfigurePanelEffects(Color accent)
    {
        if (argumentPanel == null)
            return;

        Shadow shadow = argumentPanel.GetComponent<Shadow>();
        if (shadow == null)
            shadow = argumentPanel.gameObject.AddComponent<Shadow>();

        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(0f, -8f);
        shadow.useGraphicAlpha = true;

        Outline outline = argumentPanel.GetComponent<Outline>();
        if (outline == null)
            outline = argumentPanel.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(accent.r, accent.g, accent.b, 0.55f);
        outline.effectDistance = new Vector2(1.25f, -1.25f);
        outline.useGraphicAlpha = true;
    }

    private void ConfigureButtonColors()
    {
        Button button = GetComponent<Button>();
        if (button == null)
            return;

        button.transition = Selectable.Transition.None;
        button.targetGraphic = argumentPanel;
    }

    private void ApplyPanelState()
    {
        if (argumentPanel != null)
            argumentPanel.color = isHovering ? HoverPanelColor : panelBaseColor;
    }

    private void SetFadeAlpha(float alpha)
    {
        SetColorAlpha(argumentPanel, isHovering ? HoverPanelColor : panelBaseColor, alpha);
        SetColorAlpha(accentBar, accentBaseColor, alpha);
        SetColorAlpha(iconBackplate, iconBackplateBaseColor, alpha);
        SetColorAlpha(bottomLine, bottomLineBaseColor, alpha);
        SetColorAlpha(icon, iconBaseColor, alpha);
        SetColorAlpha(argumentName, nameBaseColor, alpha);
        SetColorAlpha(description, descriptionBaseColor, alpha);
        SetColorAlpha(level, levelBaseColor, alpha);
    }

    private static void SetColorAlpha(Graphic graphic, Color baseColor, float alpha)
    {
        if (graphic == null)
            return;

        graphic.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * alpha);
    }
}
