using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private const float PanelAnimationDuration = 0.4f;
    private static readonly Color BackgroundColor = new Color(0f, 0f, 0f, 0.78f);
    private static readonly Color PanelColor = new Color(0.08f, 0.08f, 0.09f, 0.96f);
    private static readonly Color RedColor = new Color(0.88f, 0.08f, 0.08f, 1f);
    private static readonly Color GoldColor = new Color(1f, 0.72f, 0.16f, 1f);

    [Header("Root")]
    [SerializeField] private CanvasGroup backgroundGroup;
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private bool buildRuntimeLayout = true;

    [Header("Top")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    [Header("Battle Result")]
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("Augments")]
    [SerializeField] private Transform augmentRoot;
    [SerializeField] private GameObject augmentIconPrefab;

    [Header("Death And Record")]
    [SerializeField] private TextMeshProUGUI deathReasonText;
    [SerializeField] private TextMeshProUGUI currentRecordText;
    [SerializeField] private TextMeshProUGUI bestRecordText;
    [SerializeField] private TextMeshProUGUI newRecordText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button characterSelectButton;
    [SerializeField] private Button mainMenuButton;

    private Coroutine showRoutine;
    private readonly List<GameObject> spawnedAugmentIcons = new List<GameObject>();
    private float panelTargetScale = 1f;

    private void Awake()
    {
        EnsureLayout();
        BindButtons();
        Hide();
    }

    public void PrepareForSequence()
    {
        EnsureLayout();
        BindButtons();
        gameObject.SetActive(true);
        FitPanelToSafeArea();
        SetButtonsInteractable(false);

        if (backgroundGroup != null)
        {
            backgroundGroup.alpha = 0f;
            backgroundGroup.blocksRaycasts = true;
            backgroundGroup.interactable = true;
        }

        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }

        if (panelRoot != null)
            panelRoot.localScale = Vector3.one * (panelTargetScale * 0.95f);
    }

    public IEnumerator FadeBackground(float duration)
    {
        EnsureLayout();
        gameObject.SetActive(true);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(timer / duration);
            if (backgroundGroup != null)
                backgroundGroup.alpha = t;

            yield return null;
        }

        if (backgroundGroup != null)
        {
            backgroundGroup.alpha = 1f;
            backgroundGroup.blocksRaycasts = true;
            backgroundGroup.interactable = true;
        }
    }

    public void Show(ResultData result)
    {
        EnsureLayout();
        BindButtons();
        gameObject.SetActive(true);
        SetButtonsInteractable(false);
        UpdateResult(result);
        FitPanelToSafeArea();

        if (panelGroup != null)
        {
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine());
    }

    public void Hide()
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);

        if (backgroundGroup != null)
            backgroundGroup.alpha = 0f;

        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }

        if (panelRoot != null)
            panelRoot.localScale = Vector3.one * (panelTargetScale * 0.95f);

        SetButtonsInteractable(false);
        gameObject.SetActive(false);
    }

    public void UpdateResult(ResultData result)
    {
        if (result == null)
            return;

        SetText(titleText, "GAME OVER");
        SetText(survivalTimeText, FormatTime(result.SurvivalTime));
        SetText(difficultyText, result.Difficulty.ToString());
        SetText(killCountText, result.KillCount.ToString());
        SetText(levelText, result.Level.ToString());
        SetText(expText, result.Exp.ToString());
        SetText(deathReasonText, string.IsNullOrWhiteSpace(result.DeathReason) ? "Defeated by enemy attack" : result.DeathReason);
        SetText(currentRecordText, FormatTime(result.SurvivalTime));
        SetText(bestRecordText, FormatTime(result.BestTime));

        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(result.IsNewRecord);
            newRecordText.text = "NEW RECORD!";
            newRecordText.color = GoldColor;
        }

        UpdateAugments(result.Augments);
    }

    private IEnumerator ShowRoutine()
    {
        if (backgroundGroup != null)
            backgroundGroup.alpha = 1f;

        float timer = 0f;
        while (timer < PanelAnimationDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / PanelAnimationDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            if (panelGroup != null)
                panelGroup.alpha = eased;

            if (panelRoot != null)
                panelRoot.localScale = Vector3.one * Mathf.Lerp(panelTargetScale * 0.95f, panelTargetScale, eased);

            yield return null;
        }

        if (panelGroup != null)
            panelGroup.alpha = 1f;

        if (panelRoot != null)
            panelRoot.localScale = Vector3.one * panelTargetScale;

        if (panelGroup != null)
        {
            panelGroup.blocksRaycasts = true;
            panelGroup.interactable = true;
        }

        if (backgroundGroup != null)
        {
            backgroundGroup.blocksRaycasts = true;
            backgroundGroup.interactable = true;
        }

        SetButtonsInteractable(true);
        showRoutine = null;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (restartButton != null)
            restartButton.interactable = interactable;

        if (characterSelectButton != null)
            characterSelectButton.interactable = interactable;

        if (mainMenuButton != null)
            mainMenuButton.interactable = interactable;
    }

    private void BindButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(Restart);
            restartButton.onClick.AddListener(Restart);
        }

        if (characterSelectButton != null)
        {
            characterSelectButton.onClick.RemoveListener(GoCharacterSelect);
            characterSelectButton.onClick.AddListener(GoCharacterSelect);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoMainMenu);
            mainMenuButton.onClick.AddListener(GoMainMenu);
        }
    }

    private void FitPanelToSafeArea()
    {
        if (panelRoot == null || panelRoot.parent == null)
            return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRoot);

        RectTransform safeArea = panelRoot.parent as RectTransform;
        if (safeArea == null)
            return;

        Vector2 safeSize = safeArea.rect.size;
        Vector2 panelSize = panelRoot.rect.size;
        if (panelSize.x <= 0f || panelSize.y <= 0f)
            panelSize = panelRoot.sizeDelta;

        const float margin = 16f;
        float widthScale = safeSize.x > margin ? (safeSize.x - margin) / Mathf.Max(1f, panelSize.x) : 1f;
        float heightScale = safeSize.y > margin ? (safeSize.y - margin) / Mathf.Max(1f, panelSize.y) : 1f;
        panelTargetScale = Mathf.Clamp(Mathf.Min(widthScale, heightScale, 1f), 0.68f, 1f);
    }

    private void Restart()
    {
        if (!CanPressButtons())
            return;

        ResumeBeforeSceneChange();
        UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.name);
    }

    private void GoCharacterSelect()
    {
        if (!CanPressButtons())
            return;

        ResumeBeforeSceneChange();
        if (SceneManager.Instance != null)
            SceneManager.Instance.ChangeScene(SceneName.PlayerChoice);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.PlayerChoice.ToString());
    }

    private void GoMainMenu()
    {
        if (!CanPressButtons())
            return;

        ResumeBeforeSceneChange();
        if (SceneManager.Instance != null)
            SceneManager.Instance.ChangeScene(SceneName.MainMenu);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.MainMenu.ToString());
    }

    private void ResumeBeforeSceneChange()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Hide();
    }

    private bool CanPressButtons()
    {
        return panelGroup == null || (panelGroup.interactable && panelGroup.blocksRaycasts);
    }

    private void UpdateAugments(List<AugmentData> augments)
    {
        if (augmentRoot == null)
            return;

        foreach (GameObject icon in spawnedAugmentIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        spawnedAugmentIcons.Clear();

        if (augments == null || augments.Count == 0)
        {
            GameObject empty = CreateAugmentIcon("No Augments", null);
            spawnedAugmentIcons.Add(empty);
            return;
        }

        foreach (AugmentData augment in augments)
        {
            GameObject icon = CreateAugmentIcon(augment?.Name, augment?.Icon);
            spawnedAugmentIcons.Add(icon);
        }
    }

    private GameObject CreateAugmentIcon(string augmentName, Sprite icon)
    {
        GameObject obj = augmentIconPrefab != null
            ? Instantiate(augmentIconPrefab, augmentRoot)
            : BuildAugmentIcon(augmentRoot);

        obj.name = $"Augment_{(string.IsNullOrWhiteSpace(augmentName) ? "Unknown" : augmentName)}";

        Image image = obj.GetComponentInChildren<Image>();
        if (image != null)
        {
            image.sprite = icon;
            image.color = icon != null ? Color.white : new Color(0.22f, 0.22f, 0.24f, 1f);
        }

        TextMeshProUGUI label = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = string.IsNullOrWhiteSpace(augmentName) ? "-" : augmentName;

        return obj;
    }

    private void EnsureLayout()
    {
        if (!buildRuntimeLayout || panelRoot != null)
            return;

        BuildDefaultLayout();
    }

    private void BuildDefaultLayout()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        Image background = gameObject.GetComponent<Image>();
        if (background == null)
            background = gameObject.AddComponent<Image>();
        background.color = BackgroundColor;
        background.raycastTarget = true;

        backgroundGroup = GetComponent<CanvasGroup>();
        if (backgroundGroup == null)
            backgroundGroup = gameObject.AddComponent<CanvasGroup>();

        GameObject safeArea = CreateUIObject("GameOverSafeArea", transform);
        RectTransform safeRt = safeArea.GetComponent<RectTransform>();
        safeRt.anchorMin = Vector2.zero;
        safeRt.anchorMax = Vector2.one;
        safeRt.offsetMin = new Vector2(40f, 32f);
        safeRt.offsetMax = new Vector2(-40f, -32f);

        GameObject panelObj = CreateUIObject("GameOverPanel", safeArea.transform);
        panelRoot = panelObj.GetComponent<RectTransform>();
        panelRoot.anchorMin = new Vector2(0.5f, 0.5f);
        panelRoot.anchorMax = new Vector2(0.5f, 0.5f);
        panelRoot.pivot = new Vector2(0.5f, 0.5f);
        panelRoot.sizeDelta = new Vector2(900f, 900f);

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = PanelColor;

        panelGroup = panelObj.AddComponent<CanvasGroup>();
        VerticalLayoutGroup panelLayout = panelObj.AddComponent<VerticalLayoutGroup>();
        panelLayout.childAlignment = TextAnchor.UpperCenter;
        panelLayout.childControlHeight = true;
        panelLayout.childControlWidth = true;
        panelLayout.childForceExpandHeight = false;
        panelLayout.childForceExpandWidth = true;
        panelLayout.spacing = 14f;
        panelLayout.padding = new RectOffset(42, 42, 34, 34);

        ContentSizeFitter fitter = panelObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        titleText = CreateText("Title", panelRoot, "GAME OVER", 56, RedColor, FontStyles.Bold, TextAlignmentOptions.Center);
        AddSpacer(panelRoot, 2f);

        CreateSectionTitle(panelRoot, "SURVIVAL TIME");
        survivalTimeText = CreateText("SurvivalTime", panelRoot, "00:00", 34, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
        CreateSectionTitle(panelRoot, "DIFFICULTY");
        difficultyText = CreateText("Difficulty", panelRoot, "0", 28, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);

        AddDivider(panelRoot);
        CreateSectionTitle(panelRoot, "BATTLE RESULT");
        Transform battleGrid = CreateGrid("BattleResultGrid", panelRoot, 3, 132f);
        killCountText = CreateStatBlock(battleGrid, "KILLS");
        levelText = CreateStatBlock(battleGrid, "FINAL LEVEL");
        expText = CreateStatBlock(battleGrid, "EXP GAINED");

        AddDivider(panelRoot);
        CreateSectionTitle(panelRoot, "BUILD");
        GameObject augmentObj = CreateUIObject("AugmentRoot", panelRoot);
        augmentRoot = augmentObj.transform;
        HorizontalLayoutGroup augmentLayout = augmentObj.AddComponent<HorizontalLayoutGroup>();
        augmentLayout.childAlignment = TextAnchor.MiddleCenter;
        augmentLayout.childControlHeight = true;
        augmentLayout.childControlWidth = true;
        augmentLayout.childForceExpandHeight = false;
        augmentLayout.childForceExpandWidth = false;
        augmentLayout.spacing = 10f;
        LayoutElement augmentLayoutElement = augmentObj.AddComponent<LayoutElement>();
        augmentLayoutElement.minHeight = 76f;

        AddDivider(panelRoot);
        CreateSectionTitle(panelRoot, "CAUSE OF DEATH");
        deathReasonText = CreateText("DeathReason", panelRoot, "-", 24, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);

        AddDivider(panelRoot);
        Transform recordGrid = CreateGrid("RecordGrid", panelRoot, 2, 114f);
        currentRecordText = CreateStatBlock(recordGrid, "THIS RUN");
        bestRecordText = CreateStatBlock(recordGrid, "BEST RUN");
        newRecordText = CreateText("NewRecord", panelRoot, "NEW RECORD!", 32, GoldColor, FontStyles.Bold, TextAlignmentOptions.Center);

        AddSpacer(panelRoot, 2f);
        restartButton = CreateButton("Restart", panelRoot, "RETRY", new Vector2(360f, 64f), RedColor, 26);
        Transform lowerButtons = CreateGrid("LowerButtons", panelRoot, 2, 58f);
        characterSelectButton = CreateButton("CharacterSelect", lowerButtons, "CHARACTER", new Vector2(210f, 50f), new Color(0.16f, 0.16f, 0.18f, 1f), 21);
        mainMenuButton = CreateButton("MainMenu", lowerButtons, "MAIN MENU", new Vector2(210f, 50f), new Color(0.16f, 0.16f, 0.18f, 1f), 21);
    }

    private static GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static TextMeshProUGUI CreateText(string objectName, Transform parent, string text, float size, Color color, FontStyles style, TextAlignmentOptions alignment)
    {
        GameObject obj = CreateUIObject(objectName, parent);
        TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.color = color;
        label.fontStyle = style;
        label.alignment = alignment;
        label.enableAutoSizing = true;
        label.fontSizeMin = Mathf.Max(12f, size * 0.55f);
        label.fontSizeMax = size;
        label.raycastTarget = false;

        LayoutElement layout = obj.AddComponent<LayoutElement>();
        layout.minHeight = Mathf.Max(24f, size * 1.25f);
        return label;
    }

    private static void CreateSectionTitle(Transform parent, string title)
    {
        CreateText($"{title}Label", parent, title, 18, new Color(0.78f, 0.78f, 0.8f, 1f), FontStyles.Bold, TextAlignmentOptions.Center);
    }

    private static Transform CreateGrid(string objectName, Transform parent, int count, float height)
    {
        GameObject obj = CreateUIObject(objectName, parent);
        HorizontalLayoutGroup layout = obj.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 12f;

        LayoutElement element = obj.AddComponent<LayoutElement>();
        element.minHeight = height;
        element.preferredHeight = height;
        element.flexibleWidth = count;
        return obj.transform;
    }

    private static TextMeshProUGUI CreateStatBlock(Transform parent, string label)
    {
        GameObject block = CreateUIObject($"{label}Block", parent);
        Image background = block.AddComponent<Image>();
        background.color = new Color(0.12f, 0.12f, 0.14f, 0.9f);

        VerticalLayoutGroup layout = block.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = 4f;
        layout.padding = new RectOffset(8, 8, 10, 10);

        LayoutElement blockLayout = block.AddComponent<LayoutElement>();
        blockLayout.minHeight = 92f;
        blockLayout.flexibleWidth = 1f;

        CreateText($"{label}Label", block.transform, label, 17, new Color(0.75f, 0.75f, 0.78f, 1f), FontStyles.Bold, TextAlignmentOptions.Center);
        return CreateText($"{label}Value", block.transform, "0", 27, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
    }

    private static Button CreateButton(string objectName, Transform parent, string text, Vector2 size, Color color, float textSize)
    {
        GameObject obj = CreateUIObject(objectName, parent);
        Image image = obj.AddComponent<Image>();
        image.color = color;

        Button button = obj.AddComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        button.colors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f),
            pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f),
            selectedColor = Color.white,
            disabledColor = new Color(0.45f, 0.45f, 0.45f, 1f),
            colorMultiplier = 1f,
            fadeDuration = 0.08f
        };
        obj.AddComponent<GameOverButtonAnimation>();

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        LayoutElement layout = obj.AddComponent<LayoutElement>();
        layout.minWidth = size.x;
        layout.preferredWidth = size.x;
        layout.minHeight = size.y;
        layout.preferredHeight = size.y;

        TextMeshProUGUI label = CreateText("Label", obj.transform, text, textSize, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
        RectTransform labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        return button;
    }

    private static GameObject BuildAugmentIcon(Transform parent)
    {
        GameObject obj = CreateUIObject("AugmentIcon", parent);
        VerticalLayoutGroup layout = obj.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = 4f;

        LayoutElement element = obj.AddComponent<LayoutElement>();
        element.minWidth = 64f;
        element.preferredWidth = 72f;
        element.minHeight = 72f;

        GameObject iconObj = CreateUIObject("Icon", obj.transform);
        Image image = iconObj.AddComponent<Image>();
        image.preserveAspect = true;
        image.color = new Color(0.22f, 0.22f, 0.24f, 1f);
        LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
        iconLayout.minWidth = 42f;
        iconLayout.minHeight = 42f;
        iconLayout.preferredWidth = 42f;
        iconLayout.preferredHeight = 42f;

        CreateText("TooltipName", obj.transform, "-", 13, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
        return obj;
    }

    private static void AddDivider(Transform parent)
    {
        GameObject obj = CreateUIObject("Divider", parent);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.12f);

        LayoutElement layout = obj.AddComponent<LayoutElement>();
        layout.minHeight = 1f;
        layout.preferredHeight = 1f;
    }

    private static void AddSpacer(Transform parent, float height)
    {
        GameObject obj = CreateUIObject("Spacer", parent);
        LayoutElement layout = obj.AddComponent<LayoutElement>();
        layout.minHeight = height;
        layout.preferredHeight = height;
    }

    private static void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
