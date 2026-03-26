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
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }

    public void GetData(CharacterData nowData)
    {
        data = nowData;
    }
}
