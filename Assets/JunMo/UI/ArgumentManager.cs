using UnityEngine;
using System.Collections.Generic;

public class ArgumentManager : MonoBehaviour
{
    public static ArgumentManager Instance { get; private set; }
    
    [SerializeField] private ArgumentClick[] argumentClick = new ArgumentClick[3];
    [SerializeField] private ArgumentDisplay[] argumentDisplay = new ArgumentDisplay[3];
    public List<ItemData> allArguments;
    // public PlayerStats playerStats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }
    
    void OnEnable()
    {
        for (int i = 0; i < argumentClick.Length; i++)
        {
            int index = i; // 클로저 보호
            argumentClick[i].OnClick += () => OnArgument(index); //신호랑 list return
            // level up event -> Onargument()
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < argumentClick.Length; i++)
        {
            argumentClick[i].OnClick -= () => OnArgument(i);
        }
    }

    void OnArgument(int index)
    {
        argumentDisplay[index].OnClick();
    }

    public List<ItemData> RandomArgument()
    {
        List<ItemData> result = new();
        List<ItemData> list = new (allArguments);
        for (int i = 0; i < list.Count; i++)
        {
            int index = Random.Range(0, list.Count);
            result.Add(list[index]);
            list.RemoveAt(index);
        }
        return result;
    }
    
    
}