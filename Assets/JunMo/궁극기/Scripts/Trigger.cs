using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Trigger : MonoBehaviour
{
    public float lifeTime = 0.4f;
    public float fadeSpeed = 2.5f;

    private SpriteRenderer sprite;
    private Color color;

    void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();

        color = sprite.color;
        color.a = 1f;
        sprite.color = color;

        StopAllCoroutines();
        StartCoroutine(Die());
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sprite.color = color;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolManager.Instance.Release(ObjectName.Ghost, gameObject);
    }
}
