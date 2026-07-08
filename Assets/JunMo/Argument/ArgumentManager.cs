using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ArgumentManager : MonoBehaviour
{
    [SerializeField]private GameObject argumentPanel;
    [SerializeField]private Canvas canvas;

    private ArgumentDataManager upgradeManager;
    private List<Argument> arguments = new ();

    private float maxWidth = 450f;
    private int argumentCount = 0;
    private int spawnCount = 3;
    private int levelUpCnt = 0;
    private bool isChoosing = false;
    private bool isClosing = false;
    private float previousTimeScale = 1f;
    private Image argumentBackdrop;

    public int playerLevel = 1;
    void Awake()
    {
        upgradeManager = GetComponent<ArgumentDataManager>();
        ConfigureArgumentCanvas();
    }

    void OnEnable()
    {
        PlayerLevelManager.OnLevelUp += LevelUp;
    }

    void OnDisable()
    {
        PlayerLevelManager.OnLevelUp -= LevelUp;
        DestroyBackdrop();
        ResumeGame();
    }

    void Start()
    {
    }

    private void LevelUp(int currentLevel)
    {
        levelUpCnt++;
        playerLevel = currentLevel;

        if (!isChoosing)
            SpawnCalculate();
    }

    private void SpawnCalculate()
    {
        if (levelUpCnt <= 0 || isChoosing)
            return;

        levelUpCnt--;
        Spawn(spawnCount);
    }

    void Spawn(int count)
    {
        ConfigureArgumentCanvas();
        ShowBackdrop();

        isChoosing = true;
        isClosing = false;
        PauseGame();

        foreach (Argument argument in arguments)
        {
            if (argument != null)
                Destroy(argument.gameObject);
        }
        arguments.Clear();

        List<ArgumentData> datas = upgradeManager.GetRandomArguments(count);

        if (datas.Count == 0)
        {
            DestroyBackdrop();
            isChoosing = false;
            isClosing = false;
            ResumeGame();
            return;
        }

        int displayCount = datas.Count;
        float spacing = (displayCount == 1) ? 0 : maxWidth / (displayCount - 1);

        for (int i = 0; i < datas.Count; i++)
        {
            float x;
            bool centerAlign = (displayCount % 2 == 1);

            if (centerAlign)
                x = -(spacing * (displayCount - 1)) / 2f + i * spacing;
            else
                x = i * spacing - maxWidth / 2f;

            GameObject obj = Instantiate(argumentPanel, transform, false);
            RectTransform rt = obj.transform as RectTransform;

            Argument argument = obj.GetComponent<Argument>();
            argument.on_Click += OnArgumentClicked;

            argument.SetID(i);
            arguments.Add(argument);

            rt.anchoredPosition = new Vector2(x, 0);
            rt.localScale = Vector3.one;
            rt.sizeDelta = ((RectTransform)argumentPanel.transform).sizeDelta;

            ArgumentData data = datas[i];

            if (data is SkillArgumentData skill)
            {
                int level = upgradeManager.GetSkillLevel(skill);
                argument.SetSkill(skill, level + 1);
            }
            else if (data is StatArgumentData stat)
                argument.SetStat(stat);
        }
    }

    void OnArgumentClicked(int argumentID)
    {
        if (isClosing)
            return;

        ArgumentData data = upgradeManager.MakeArgument(argumentID);

        if (data == null || argumentID < 0 || argumentID >= arguments.Count)
            return;

        isClosing = true;

        foreach (Argument clickedArgument in arguments)
        {
            if (clickedArgument == arguments[argumentID])
            {
                arguments[argumentID].FadeOut(1f);

                if (data is SkillArgumentData skill)
                {
                    upgradeManager.ApplySkillResult(skill);
                }

                upgradeManager.ConvertToResult(data);
            }
            else
            {
                clickedArgument.FadeOut(0.3f);
            }
        }

        StartCoroutine(CloseAndShowNextRoutine());
    }

    private IEnumerator CloseAndShowNextRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        arguments.Clear();
        isChoosing = false;
        isClosing = false;

        if (levelUpCnt > 0)
        {
            SpawnCalculate();
        }
        else
        {
            DestroyBackdrop();
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        if (Time.timeScale > 0f)
            previousTimeScale = Time.timeScale;

        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;
    }

    private void ConfigureArgumentCanvas()
    {
        if (canvas == null)
            return;

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
    }

    private void ShowBackdrop()
    {
        if (canvas == null)
            return;

        if (argumentBackdrop == null)
        {
            GameObject obj = new GameObject(
                "ArgumentBackdrop",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            obj.layer = canvas.gameObject.layer;
            obj.transform.SetParent(canvas.transform, false);
            obj.transform.SetAsFirstSibling();

            RectTransform rect = obj.transform as RectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            argumentBackdrop = obj.GetComponent<Image>();
            argumentBackdrop.color = new Color(0.015f, 0.02f, 0.035f, 0.72f);
            argumentBackdrop.raycastTarget = true;
        }

        argumentBackdrop.gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    private void DestroyBackdrop()
    {
        if (argumentBackdrop == null)
            return;

        Destroy(argumentBackdrop.gameObject);
        argumentBackdrop = null;
    }
}
