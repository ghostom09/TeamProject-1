using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    private ObjectName poolKey;

    private Coroutine autoReleaseCoroutine;
    private bool isReleased;

    public void Init(ObjectName key)
    {
        poolKey = key;
        isReleased = false;
    }

    public void AutoRelease(float delay)
    {
        if (delay <= 0f)
            return;

        if (autoReleaseCoroutine != null)
            StopCoroutine(autoReleaseCoroutine);

        autoReleaseCoroutine = StartCoroutine(AutoReleaseRoutine(delay));
    }

    private IEnumerator AutoReleaseRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (isReleased)
            return;

        isReleased = true;

        if (ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.Release(poolKey, gameObject);
    }

    private void OnDisable()
    {
        if (autoReleaseCoroutine != null)
        {
            StopCoroutine(autoReleaseCoroutine);
            autoReleaseCoroutine = null;
        }
    }
}