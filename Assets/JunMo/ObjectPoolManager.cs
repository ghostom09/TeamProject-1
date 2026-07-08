using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public enum ObjectName
{
    NormalBullet,
    GoldenBullet,
    UltraBullet,
    Ghost,
    Boom,
    
    SwordIllusions,
    PlayerAvatar,
    UltraSwordIllusions,
    UltraSwordBigIllusions,
    UltraSwordFinal,
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolData
    {
        public ObjectName key;
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 100;
        public bool prewarm = true;
    }

    [SerializeField] private List<PoolData> poolDataList = new();

    private Dictionary<ObjectName, ObjectPool<GameObject>> pools = new();
    private Dictionary<ObjectName, GameObject> prefabMap = new();
    private Dictionary<ObjectName, Transform> parentMap = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitPools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitPools()
    {
        foreach (var data in poolDataList)
        {
            RegisterPool(data);
        }
    }

    public void RegisterPool(PoolData data)
    {
        if (pools.ContainsKey(data.key))
            return;

        prefabMap[data.key] = data.prefab;

        Transform parent = new GameObject($"Pool_{data.key}").transform;
        parent.SetParent(transform);
        parentMap[data.key] = parent;

        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefabMap[data.key], parentMap[data.key]);
                obj.GetComponent<PooledObject>()?.Init(data.key);
                return obj;
            },

            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
            },

            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(parentMap[data.key]);
            },

            actionOnDestroy: (obj) =>
            {
                Destroy(obj);
            },

            collectionCheck: Application.isEditor,
            defaultCapacity: data.defaultCapacity,
            maxSize: data.maxSize
        );

        pools[data.key] = pool;

        if (data.prewarm)
            Prewarm(data.key, data.defaultCapacity);
    }

    private void Prewarm(ObjectName key, int count)
    {
        if (!pools.TryGetValue(key, out var pool))
            return;

        List<GameObject> temp = new();

        for (int i = 0; i < count; i++)
            temp.Add(pool.Get());

        foreach (var obj in temp)
            pool.Release(obj);
    }

    public GameObject Get(ObjectName key, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"Pool '{key}' does not exist.");
            return null;
        }

        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public void Release(ObjectName key, GameObject obj)
    {
        if (obj == null)
            return;

        if (!pools.TryGetValue(key, out var pool))
        {
            Debug.LogWarning($"Pool '{key}' not found. Destroying object.");
            Destroy(obj);
            return;
        }

        pool.Release(obj);
    }
}