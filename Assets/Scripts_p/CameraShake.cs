using UnityEngine;

[DefaultExecutionOrder(1000)]
public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;

    private float duration;
    private float intensity;
    private Vector3 lastOffset;

    public static void Shake(float power, float time)
    {
        CameraShake shake = GetOrCreate();
        if (shake == null)
            return;

        shake.intensity = Mathf.Max(shake.intensity, power);
        shake.duration = Mathf.Max(shake.duration, time);
    }

    private static CameraShake GetOrCreate()
    {
        if (instance != null)
            return instance;

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return null;

        instance = mainCamera.GetComponent<CameraShake>();
        if (instance == null)
            instance = mainCamera.gameObject.AddComponent<CameraShake>();

        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        if (lastOffset != Vector3.zero)
        {
            transform.position -= lastOffset;
            lastOffset = Vector3.zero;
        }

        if (duration <= 0f)
            return;

        duration -= Time.unscaledDeltaTime;
        float fade = Mathf.Clamp01(duration / 0.18f);
        Vector2 offset = Random.insideUnitCircle * (intensity * fade);
        lastOffset = new Vector3(offset.x, offset.y, 0f);
        transform.position += lastOffset;

        if (duration <= 0f)
            intensity = 0f;
    }
}
