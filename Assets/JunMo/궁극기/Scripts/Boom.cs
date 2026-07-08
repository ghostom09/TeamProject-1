using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour
{
    private float lifeTime = 0.2f;
    Coroutine dieCoroutine;

    void OnEnable()
    {
        if (dieCoroutine != null)
            StopCoroutine(dieCoroutine);

        dieCoroutine = StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolManager.Instance.Release(ObjectName.Boom, gameObject);
    }
}
