using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public ArgumentManager argumentManager;
    public int spawnCount = 4;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        argumentManager.Spawn(spawnCount);
    }
}
