using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float magnitude = 0.2f;

    float timeLeft;
    Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float power, float duration)
    {
        magnitude = power;
        timeLeft = duration;
    }

    void Update()
    {
        // Shake(0.1f);
        if (timeLeft > 0)
        {
            transform.localPosition =
                originalPos + Random.insideUnitSphere * magnitude;

            timeLeft -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }
}
