using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InGameUIManager : MonoBehaviour
{
    public event Action on_esc;
    public InputActionReference cancelAction;
    
    [SerializeField] private Image healthBar;
    [SerializeField] private Image backBar;
    [SerializeField] private Image experienceBar;
    [SerializeField] private Image ultraBar;
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject skillPanel;
    [SerializeField] Canvas canvas;
    [SerializeField] private GameObject deadBackground;
    [SerializeField] private int deadCanvasSortingOrder = 32767;
    [SerializeField] private int magicianUltimateSortingOrder = 32766;
    
    private float lerpSpeed = 20f;
    private GameObject magicianUltimateEdgeCanvas;
    private Image magicianUltimateEdge;
    private Coroutine magicianUltimateEffectRoutine;
    
    private float displayedHealth;
    private float displayedExperience;
    private float displayedUltra;
    
    private Coroutine delayRoutine;

    private List<SkillTimer> skillTimers = new();
    private int spawnCount = 3;
    private Player player;
    private PlayerLevelManager levelManager;
    
    private void Awake()
    {
        displayedHealth = 0;
        displayedUltra = 0;
        displayedExperience = 0;

        UpdateLevel(1);
        UpdateUltimate(0, 100);
        UpdateExperience(0, 100);
        SetDeadBackground(false);
        EnsureMagicianUltimateEdge();
        
        Spawn(spawnCount);
    }

    void Start()
    {
        UIManager.Instance.SetHUD(this);
        UIManager.Instance.UpdateInGameUI();
        BindPlayerUI();
    }
    void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnStop;
        SubscribePlayerUI();
    }

    void OnDisable()
    {
        cancelAction.action.performed -= OnStop;
        cancelAction.action.Disable();
        UnsubscribePlayerUI();

        if (magicianUltimateEffectRoutine != null)
            StopCoroutine(magicianUltimateEffectRoutine);

        magicianUltimateEffectRoutine = null;
        HideMagicianUltimateEffect();
    }

    void OnStop(InputAction.CallbackContext context)
    {
        on_esc?.Invoke();
    }

    public void UpdateLevel(int level)
    {
        levelText.SetText($"Level : {level}");
    }

    public void UpdateProfile(Sprite sprite)
    {
        profile.sprite = sprite;
    }
    
    private void BindPlayerUI()
    {
        player = FindObjectOfType<Player>();
        levelManager = player != null ? player.GetComponent<PlayerLevelManager>() : FindObjectOfType<PlayerLevelManager>();

        SubscribePlayerUI();

        if (player != null)
        {
            UpdateHealth(player.Stats.currentHp, player.Stats.MaxHp);
            UpdateUltimate(player.currentGauge, player.maxGauge);
        }

        if (levelManager != null)
        {
            UpdateLevel(levelManager.CurrentLevel);
            UpdateExperience(levelManager.CurrentExp, levelManager.RequiredExp);
        }
    }

    private void SubscribePlayerUI()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealth;
            player.OnGaugeChanged -= UpdateUltimate;
            player.OnHealthChanged += UpdateHealth;
            player.OnGaugeChanged += UpdateUltimate;
        }

        if (levelManager != null)
        {
            levelManager.OnExperienceChanged -= UpdateExperience;
            levelManager.OnExperienceChanged += UpdateExperience;
            PlayerLevelManager.OnLevelUp -= UpdateLevel;
            PlayerLevelManager.OnLevelUp += UpdateLevel;
        }
    }

    private void UnsubscribePlayerUI()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealth;
            player.OnGaugeChanged -= UpdateUltimate;
        }

        if (levelManager != null)
        {
            levelManager.OnExperienceChanged -= UpdateExperience;
            PlayerLevelManager.OnLevelUp -= UpdateLevel;
        }
    }

    public void UpdateHealth(float current, float max)
    {
        if (max <= 0f)
        {
            healthBar.fillAmount = 0f;
            backBar.fillAmount = 0f;
            displayedHealth = 0f;
            return;
        }

        float targetHP = Mathf.Clamp(current, 0, max);

        healthBar.fillAmount = targetHP / max;
        SetDeadBackground(targetHP <= 0f);

        if (displayedHealth <= 0) displayedHealth = max;

        if (delayRoutine != null)
        {
            StopCoroutine(delayRoutine);
        }

        delayRoutine = StartCoroutine(DelayedLerp(max, targetHP));
    }

    private IEnumerator DelayedLerp(float max, float targetHP)
    {
        yield return new WaitForSeconds(0.3f);

        while (Mathf.Abs(displayedHealth - targetHP) > 0.01f)
        {
            float diffRatio = Mathf.Abs(displayedHealth - targetHP) / max;

            float dynamicSpeed = lerpSpeed * Mathf.Lerp(1f, 3f, diffRatio);  

            displayedHealth = Mathf.MoveTowards(
                displayedHealth, 
                targetHP, 
                dynamicSpeed * Time.deltaTime
            );

            backBar.fillAmount = displayedHealth / max;
            yield return null;
        }

        delayRoutine = null;
    }

    private void SetDeadBackground(bool isActive)
    {
        if (deadBackground == null)
            return;

        ConfigureDeadCanvas();
        deadBackground.SetActive(isActive);
        deadBackground.transform.localScale = isActive ? Vector3.one : Vector3.zero;

        if (isActive)
        {
            deadBackground.transform.SetAsLastSibling();
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
    }

    private void ConfigureDeadCanvas()
    {
        if (!deadBackground.TryGetComponent(out Canvas deadCanvas))
            return;

        deadCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        deadCanvas.worldCamera = null;
        deadCanvas.overrideSorting = true;
        deadCanvas.sortingOrder = deadCanvasSortingOrder;
    }

    public void UpdateExperience(int currentExp, int max)
    {
        if (max <= 0)
        {
            experienceBar.fillAmount = 0f;
            displayedExperience = 0f;
            return;
        }

        displayedExperience = Mathf.Clamp(currentExp, 0, max);
        experienceBar.fillAmount = displayedExperience / (float)max;
    }
    
    public void UpdateUltimate(float currentGauge, float max)
    {
        if (max <= 0f)
        {
            ultraBar.fillAmount = 0f;
            displayedUltra = 0f;
            return;
        }

        displayedUltra = Mathf.Clamp(currentGauge, 0, max);
    
        ultraBar.fillAmount = displayedUltra / max;
    }

    public void UpdateSkillTime(int skillTime1, int skillTime2, int skillTime3)
    {
        skillTimers[0].Timer(skillTime1);
        skillTimers[1].Timer(skillTime2);
        skillTimers[2].Timer(skillTime3);
    }

    public void UpdateSkillTime(SkillData[] skills)
    {
        if (skills == null)
            return;

        int count = Mathf.Min(skillTimers.Count, skills.Length);
        for (int i = 0; i < count; i++)
        {
            skillTimers[i].Timer(Mathf.CeilToInt(skills[i].Cooldown));
        }
    }

    public void UpdateSkillTimer(int cnt)
    {
        if (cnt < 0 || cnt >= skillTimers.Count)
            return;

        skillTimers[cnt].UseSkill();
    }

    public void ShowMagicianUltimateEffect(float duration)
    {
        EnsureMagicianUltimateEdge();

        if (magicianUltimateEdge == null)
            return;

        if (magicianUltimateEffectRoutine != null)
            StopCoroutine(magicianUltimateEffectRoutine);

        if (magicianUltimateEdgeCanvas != null)
            magicianUltimateEdgeCanvas.SetActive(true);

        magicianUltimateEffectRoutine = StartCoroutine(MagicianUltimateEffectRoutine(duration));
    }

    public void HideMagicianUltimateEffect()
    {
        if (magicianUltimateEffectRoutine != null)
        {
            StopCoroutine(magicianUltimateEffectRoutine);
            magicianUltimateEffectRoutine = null;
        }

        if (magicianUltimateEdge != null)
            magicianUltimateEdge.gameObject.SetActive(false);

        if (magicianUltimateEdgeCanvas != null)
            magicianUltimateEdgeCanvas.SetActive(false);
    }

    private IEnumerator MagicianUltimateEffectRoutine(float duration)
    {
        if (magicianUltimateEdgeCanvas != null)
            magicianUltimateEdgeCanvas.SetActive(true);

        magicianUltimateEdge.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float pulse = Mathf.Lerp(0.8f, 1f, (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f);
            magicianUltimateEdge.color = new Color(0.7f * pulse, 0f, 1f * pulse, 1f);
            yield return null;
        }

        magicianUltimateEdge.gameObject.SetActive(false);
        if (magicianUltimateEdgeCanvas != null)
            magicianUltimateEdgeCanvas.SetActive(false);

        magicianUltimateEffectRoutine = null;
    }

    private void EnsureMagicianUltimateEdge()
    {
        if (magicianUltimateEdge != null)
            return;

        magicianUltimateEdgeCanvas = new GameObject(
            "MagicianUltimateEdgeCanvas",
            typeof(RectTransform),
            typeof(Canvas)
        );
        magicianUltimateEdgeCanvas.transform.SetParent(transform, false);

        RectTransform canvasRt = magicianUltimateEdgeCanvas.GetComponent<RectTransform>();
        canvasRt.anchorMin = Vector2.zero;
        canvasRt.anchorMax = Vector2.one;
        canvasRt.offsetMin = Vector2.zero;
        canvasRt.offsetMax = Vector2.zero;

        Canvas edgeCanvas = magicianUltimateEdgeCanvas.GetComponent<Canvas>();
        edgeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        edgeCanvas.overrideSorting = true;
        edgeCanvas.sortingOrder = magicianUltimateSortingOrder;

        GameObject obj = new GameObject("MagicianUltimateEdge", typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(magicianUltimateEdgeCanvas.transform, false);

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        magicianUltimateEdge = obj.GetComponent<Image>();
        magicianUltimateEdge.sprite = CreateEdgeSprite();
        magicianUltimateEdge.type = Image.Type.Sliced;
        magicianUltimateEdge.raycastTarget = false;
        magicianUltimateEdge.color = new Color(0.7f, 0f, 1f, 1f);
        magicianUltimateEdge.gameObject.SetActive(false);
        magicianUltimateEdgeCanvas.SetActive(false);
    }

    private Sprite CreateEdgeSprite()
    {
        const int size = 96;
        const int border = 30;
        const int solidBorder = 10;

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int edgeDistance = Mathf.Min(Mathf.Min(x, y), Mathf.Min(size - 1 - x, size - 1 - y));
                float alpha = edgeDistance <= solidBorder
                    ? 1f
                    : Mathf.Clamp01(1f - (edgeDistance - solidBorder) / (float)(border - solidBorder));
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(border, border, border, border)
        );
    }

    private void Spawn(int count)
    {
        const float spacing = 147f;
        const float rightMargin = 40f;
        const float bottomMargin = 40f;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(skillPanel, canvas.transform, false);
            RectTransform rt = obj.transform as RectTransform;
            SkillTimer skillTimer = obj.GetComponent<SkillTimer>();
            skillTimers.Add(skillTimer);
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-rightMargin - (count - 1 - i) * spacing, bottomMargin);
            rt.localScale = Vector3.one;
            rt.sizeDelta = ((RectTransform)skillPanel.transform).sizeDelta;
        }
    }
}
