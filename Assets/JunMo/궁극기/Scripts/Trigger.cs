using UnityEngine;

public class Trigger : MonoBehaviour
{
    public float lifeTime = 0.4f;
    public float fadeSpeed = 2.5f;

    private SpriteRenderer sprite;
    private Color color;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        color = sprite.color;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sprite.color = color;
    }
}
