using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ArgumentManager : MonoBehaviour
{
    [SerializeField]private UpgradeManager upgradeManager;
    [SerializeField]private Argument argument;
    [SerializeField]private GameObject argumentPanel;
    private List<Argument> arguments = new ();
    // levelup event

    private float maxWidth = 1000f;
    private int spawnCount = 3;
    private int argumentCount = 0;
    
    void Start()
    {
        Spawn(spawnCount);
    }
    void Spawn(int count)
    {
        arguments.Clear();
        float spacing = (count == 1) ? 0 : maxWidth / (count - 1);
        for (int i = 0; i < count; i++)
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
            rt.anchoredPosition = new Vector2(x, 0);
            rt.localScale = Vector3.one;
            rt.sizeDelta = ((RectTransform)argumentPanel.transform).sizeDelta;
            argument.SetID(argumentCount++);
            if (upgradeManager.RandomType())
                argument.SetStat(upgradeManager.RandomStat());
            else
                argument.SetSkill(upgradeManager.RandomSkill());
        }
    }

    
    void OnArgumentClicked(int argumentID)
    {
        foreach (Argument clickedArgument in arguments)
        {
            if (clickedArgument == arguments[argumentID])
            {
                arguments[argumentID].FadeOut(1f);
            }
            else clickedArgument.FadeOut(0.3f);
        }
    }
}
