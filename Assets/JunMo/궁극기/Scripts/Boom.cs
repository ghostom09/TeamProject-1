using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour
{
    private float lifeTime = 0.2f;
    void Start()
    {
        StopCoroutine(Die());
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolManager.Instance.Release(ObjectName.Boom, gameObject);
    }
}
