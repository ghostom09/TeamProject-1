using UnityEngine;

public class CharDataManager : MonoBehaviour
{
    public CharacterData data;
    public static CharDataManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void GetData(CharacterData nowData)
    {
        data = nowData;
    }
}
