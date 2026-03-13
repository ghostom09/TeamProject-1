using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class ArgumentManager : MonoBehaviour
{
    [SerializeField]private GameObject argumentPanel;
    [SerializeField]private Canvas canvas;
    
    private ArgumentDataManager upgradeManager;
    private List<Argument> arguments = new ();

    private float maxWidth = 450f;
    private int argumentCount = 0;

    public int spawnCount = 3;
    void Awake()
    {
        upgradeManager = GetComponent<ArgumentDataManager>();
    }
    
    void Start()
    {
        Spawn(spawnCount);
    }
    
    void Spawn(int count)
    {
        arguments.Clear();

        List<ArgumentData> datas = upgradeManager.GetRandomArguments(count);

        float spacing = (count == 1) ? 0 : maxWidth / (count - 1);

        for (int i = 0; i < datas.Count; i++)
        {
            float x;
            bool centerAlign = (count % 2 == 1);

            if (centerAlign)
                x = -(spacing * (count - 1)) / 2f + i * spacing;
            else
                x = i * spacing - maxWidth / 2f;

            GameObject obj = Instantiate(argumentPanel, transform, false);
            RectTransform rt = obj.transform as RectTransform;

            Argument argument = obj.GetComponent<Argument>();
            argument.on_Click += OnArgumentClicked;

            arguments.Add(argument);
            argument.SetID(i);

            rt.anchoredPosition = new Vector2(x, 0);
            rt.localScale = Vector3.one;
            rt.sizeDelta = ((RectTransform)argumentPanel.transform).sizeDelta;

            ArgumentData data = datas[i];

            if (data is SkillArgumentData skill)
            {
                int level = upgradeManager.GetSkillLevel(skill) + 1;
                argument.SetSkill(skill, level);
            }
            else if (data is StatArgumentData stat)
                argument.SetStat(stat);
        }
    }

    
    void OnArgumentClicked(int argumentID)
    {
        ArgumentData data = upgradeManager.MakeArgument(argumentID);

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
    }
    
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Spawn(spawnCount);
        }
    }
}
