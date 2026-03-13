using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public enum objectName{
    Argument,
    
}
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    
    [System.Serializable]
    public class PoolData
    {
        public objectName key;
        public GameObject prefab;
        public int defaultCapacity = 1;
        public int maxSize = 100;
    }

    [SerializeField] private List<PoolData> poolDataList = new();

    private Dictionary<objectName, ObjectPool<GameObject>> _pools = new();
    private Dictionary<objectName, GameObject> _prefabMap = new();
    private Dictionary<objectName, Transform> _parentMap = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);

        InitPools();
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
        if (_pools.ContainsKey(data.key)) return;
        _prefabMap[data.key] = data.prefab;

        var parent = new GameObject($"Pool_{data.key}").transform;
        parent.SetParent(transform);
        _parentMap[data.key] = parent;
        
        var pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Instantiate(_prefabMap[data.key], _parentMap[data.key]);
                obj.GetComponent<PooledObject>()?.Init(data.key);
                return obj;
            },
            actionOnGet:     obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: Application.isEditor,
            defaultCapacity: data.defaultCapacity,
            maxSize:         data.maxSize
        );

        _pools[data.key] = pool;
    }
    
    public GameObject Get(objectName key)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"[PoolManager] '{key}' 풀이 없습니다!");
            return null;
        }
        return pool.Get();
    }

    public void Release(objectName key, GameObject obj)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Destroy(obj);
            return;
        }
        pool.Release(obj);
    }
}