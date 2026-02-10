using UnityEngine;
using System.Collections.Generic;

public class ArgumentManager : MonoBehaviour
{
    public static ArgumentManager Instance { get; private set; }

    public ArgumentsCalculate calculator = new();
    // public PlayerStats playerStats;
    [SerializeField]private ArgumentClick argumentClick;
    [SerializeField]private ArgumentDisplay argumentDisplay;
    
    public List<ItemData> allArguments; //모든 증강 넣어두는 곳
    public List<ArgumentDisplay> nowArguments; //지금증강들
    
    [SerializeField] private GameObject ArgumentCard;
    public float maxWidth = 1000f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
        
    }
    
    public void Spawn(int count)
    {
        nowArguments.Clear();
        float spacing = (count == 1) ? 0 : maxWidth / (count - 1);
        for (int i = 0; i < count; i++)
        {
            float x;
            bool centerAlign = (count % 2 == 1);

            if (centerAlign)
                x = -(spacing * (count - 1)) / 2f + i * spacing;
            else
                x = i * spacing - maxWidth / 2f;

            GameObject obj = Instantiate(ArgumentCard, gameObject.transform, false);
            RectTransform rt = obj.transform as RectTransform;
            ArgumentDisplay display = obj.GetComponent<ArgumentDisplay>();
            nowArguments.Add(display);
            rt.anchoredPosition = new Vector2(x, 0);
            rt.localScale = Vector3.one;
            rt.sizeDelta = ((RectTransform)ArgumentCard.transform).sizeDelta;
            display.SetArgument(RandomArgument());
        }
    }
    
    public ItemData RandomArgument() // 랜덤하게 하나 줌
    {
        if (allArguments == null || allArguments.Count == 0)
            return null;

        int index = Random.Range(0, allArguments.Count);
        return allArguments[index];
    }

    public void Calculate(ItemData items) // 지금 무슨 증강인지
    {
        calculator.CalculateArguments(items);
    }

    public void OnArgumentClick(ArgumentDisplay selectedDisplay)
    {
        foreach (var display in nowArguments)
        {
            if (display == selectedDisplay)
            {
                display.FadeOut(1f);
            }
            else
            {
                display.FadeOut(0.3f);
            }
        }
    }
}